using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Link
{
    public class DiscordEventHandler
    {
        public static async Task MessageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            if (msg.Value.Author.IsBot) return;
            if (channel is IDMChannel) return;

            // Check to see if this isn't me
            if (ClearService.CurrentClears.Contains(channel.Id)) return;

            var _channel = channel as IGuildChannel;
            var _logChannel = LogService.GetLogChannel(_channel.Guild.Id);

            if (_logChannel == null) return;

            await _logChannel.SendMessageAsync($":x: **MESSAGE DELETED:**  ({channel.Name}) {msg.Value.Author.Username}#{msg.Value.Author.Discriminator}| `{msg.Value.Content}`");
        }

        public static async Task MessageUpdated(Cacheable<IMessage, ulong> oldMessage, SocketMessage newMessage, ISocketMessageChannel channel)
        {
            if (oldMessage.Value.Author.IsBot) return;
            if (channel is IDMChannel) return;

            var _channel = channel as IGuildChannel;
            var _logChannel = LogService.GetLogChannel(_channel.Guild.Id);

            if (_logChannel == null) return;

            await _logChannel.SendMessageAsync($":pencil2: **MESSAGE UPDATED:** {oldMessage.Value.Author.Username}#{oldMessage.Value.Author.Discriminator} " +
                $"  \nOLD: {oldMessage.Value.Content}" +
                $"  \nNEW: {newMessage.Content}");
        }

        public static async Task UserLeft(SocketGuildUser user)
        {
            var _logChannel = LogService.GetLogChannel(user.Guild.Id);
            if (_logChannel == null) return;

            var _embed = new EmbedBuilder()
                .WithTitle($"User left: {user.Username}#{user.Discriminator}")
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithColor(Color.Blue)
                .WithDescription($"\nCreated: {user.CreatedAt}" +
                                 $"\nIsBot: {user.IsBot}" +
                                 $"\nID: {user.Id}");

            await _logChannel.SendMessageAsync("", false, _embed.Build());
        }

        public static async Task UserJoined(SocketGuildUser user)
        {
            await MuteService.CheckNewUserForMute(user);
            await BanService.CheckNewUserForBan(user);

            var _logChannel = LogService.GetLogChannel(user.Guild.Id);
            if (_logChannel == null) return;

            var _embed = new EmbedBuilder()
                .WithTitle($"User joined: {user.Username}#{user.Discriminator}")
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithColor(Color.Blue)
                .WithDescription($"\nCreated: {user.CreatedAt}" +
                                 $"\nIsBot: {user.IsBot}" +
                                 $"\nID: {user.Id}");

            await _logChannel.SendMessageAsync("", false, _embed.Build());
        }

        public static async Task JoinedGuild(SocketGuild guild)
        {
            LogService.Log.Information($"Joined guild: {guild.Name}.");

            // Create guild configuration
            Database.CreateDefaultGuildConfig(guild, out _);
        }

        public static async Task LeftGuild(SocketGuild guild)
        {
            LogService.Log.Information($"Left guild: {guild.Name}.");

            // Delete guild configuration
            Database.DeleteRecord<GuildConfig>(s => s.ID == guild.Id);
        }

        public static async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            // Get guild ID
            var _guildUser = user as IGuildUser;
            if (_guildUser == null) return;

            if (before.VoiceChannel == null && after.VoiceChannel != null) // If joins channel
            {
                await MuteService.CheckNewUserForVoiceMute(user as IGuildUser);
            }

            if (!before.IsMuted && after.IsMuted) // If server muted
            {
                MuteService.AddVoiceMute(_guildUser.Id, _guildUser.GuildId);
            }
            else if(before.IsMuted && !after.IsMuted) // If un-server muted
            {
                MuteService.RemoveVoiceMute(_guildUser.Id, _guildUser.GuildId);
            }
        }

        public static async Task GuildMemberUpdated(SocketGuildUser before, SocketGuildUser after)
        {
            IGuildUser _before = before as IGuildUser;
            IGuildUser _after = after as IGuildUser;
            if (_before == null || _after == null) return;

            if (_before.RoleIds != _after.RoleIds) // Role removed or added
            {
                // Check if muted role was removed or added
                var _mutedRole = await MuteService.GetOrCreateMutedRoleAsync(_before.Guild);
                if (_before.RoleIds.Contains(_mutedRole.Id) && !_after.RoleIds.Contains(_mutedRole.Id))
                {
                    MuteService.RemoveMute(_after);
                }
                else if (!_before.RoleIds.Contains(_mutedRole.Id) && _after.RoleIds.Contains(_mutedRole.Id))
                {
                    MuteService.AddMute(_after);
                }
            }
        }

        public static async Task ClientReady()
        {
            LogService.Log.Information($"Connected as: {Program.client.CurrentUser.Username}#{Program.client.CurrentUser.Discriminator}");

            new CommandHandlingService(Program.client);
            new ConfirmationService();

            Database.CheckForAllGuildConfigs();
        }

        public static async Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Debug:
                    LogService.Log.Debug($"({msg.Source}) {msg.Message}");
                    break;
                case LogSeverity.Verbose:
                    LogService.Log.Verbose($"({msg.Source}) {msg.Message}");
                    break;
                case LogSeverity.Info:
                    LogService.Log.Information($"({msg.Source}) {msg.Message}");
                    break;
                case LogSeverity.Warning:
                    LogService.Log.Warning($"({msg.Source}) {msg.Message}");
                    break;
                case LogSeverity.Error:
                    LogService.Log.Error($"({msg.Source}) {msg.Message}");
                    break;
                case LogSeverity.Critical:
                    LogService.Log.Fatal($"({msg.Source}) {msg.Message}");
                    break;
            }

            Database.UpsertRecord(new LogRecord()
            {
                Date = DateTime.Now,
                Log = msg.Message,
                Type = LogType.bot
            });
        }
    }
}
