using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reactive;

namespace Link
{
    public class MuteService
    {
        public static async Task Mute(SocketCommandContext Context, IGuildUser user)
        {
            var _role = await GetOrCreateMutedRoleAsync(Context.Guild);

            await user.AddRoleAsync(_role);

            await Respond.SendResponse(Context, $"Muted user **{user.Username}#{user.Discriminator}**.");

            AddMuteToDatabase(user);
        }

        public static async Task Mute(SocketCommandContext Context, ulong id)
        {
            var _user = Context.Guild.GetUser(id);
            if (_user == null)
            {
                await Respond.SendResponse(Context, $"I could not find a user with the ID: `{id}`.");
                return;
            }

            var _role = await GetOrCreateMutedRoleAsync(Context.Guild);

            await _user.AddRoleAsync(_role);

            await Respond.SendResponse(Context, $"Muted user **{_user.Username}#{_user.Discriminator}**.");

            AddMuteToDatabase(_user);
        }

        public static async Task Mute(SocketCommandContext Context, IGuildUser user, string time)
        {
            var _role = await GetOrCreateMutedRoleAsync(Context.Guild);

            await user.AddRoleAsync(_role);

            var _time = Parse.Time(time);

            if (_time.TotalDays == 0) _time = TimeSpan.FromMinutes(30);

            AddMuteToDatabase(user, _time);

            await Respond.SendResponse(Context, $"Muted user **{user.Username}#{user.Discriminator}** for {time}.");
        }

        public static async Task Unmute(SocketCommandContext Context, IGuildUser user)
        {
            var _role = await GetOrCreateMutedRoleAsync(Context.Guild);

            if (!user.RoleIds.Contains(_role.Id))
            {
                await Respond.SendResponse(Context, $"{user.Username}#{user.Discriminator} is already unmuted!");
                return;
            }

            await user.RemoveRoleAsync(_role);

            await Respond.SendResponse(Context, $"Unmuted {user.Username}#{user.Discriminator}.");

            RemoveMuteFromDatabase(user);
        }

        public static async Task Unmute(SocketCommandContext Context, ulong id)
        {
            var _role = await GetOrCreateMutedRoleAsync(Context.Guild);

            var _user = Context.Guild.GetUser(id);
            if (_user == null)
            {
                await Respond.SendResponse(Context, $"I could not find a user with the ID: `{id}`");
                return;
            }

            if (!_user.Roles.Any(s => s.Id == _role.Id))
            {
                await Respond.SendResponse(Context, $"{_user.Username}#{_user.Discriminator} is already unmuted!");
                return;
            }

            await _user.RemoveRoleAsync(_role);

            await Respond.SendResponse(Context, $"Unmuted {_user.Username}#{_user.Discriminator}.");

            RemoveMuteFromDatabase(_user);
        }

        public static async Task CheckNewUserForMute(IGuildUser user)
        {
            var _record = Database.GetRecord<MuteRecord>(s => s.GuildId == user.Guild.Id && s.UserId == user.Id);

            if (_record != null)
            {
                await MuteNewUser(user);
            }
        }

        public static async Task CheckNewUserForVoiceMute(IGuildUser user)
        {
            var _record = Database.GetRecord<VoiceMuteRecord>(s => s.UserId == user.Id && s.GuildId == user.GuildId);

            if (_record != null)
            {
                await VoiceMuteUser(user);
            }
        }

        public static async Task MuteNewUser(IGuildUser user)
        {
            // Get muted role
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == user.GuildId);
            var _role = user.Guild.GetRole(_config.MutedRoleID);

            await user.AddRoleAsync(_role);
        }

        public static async Task UpdateDatabaseTimers()
        {
            var _records = Database.GetRecords<MuteRecord>(s => s.Time != null);

            if (_records == null) return;

            foreach (var mute in _records)
            {
                mute.Time = mute.Time.Value.Subtract(TimeSpan.FromSeconds(2));

                if (mute.Time.Value.TotalSeconds <= 0)
                {
                    await UnmuteUserFromID(mute.GuildId, mute.UserId);
                }
                else
                {
                    Database.UpsertRecord(mute);
                    continue;
                }
            }
        }

        public static void AddMuteToDatabase(IGuildUser user, TimeSpan? time = null)
        {
            var _record = new MuteRecord()
            {
                GuildId = user.GuildId,
                UserId = user.Id,
                Time = time
            };

            Database.UpsertRecord(_record);
        }

        public static void RemoveMuteFromDatabase(IGuildUser user)
        {
            var _record = Database.GetRecord<MuteRecord>(s => s.GuildId == user.GuildId && s.UserId == user.Id);

            if (_record != null)
            {
                Database.DeleteRecord<MuteRecord>(s => s.GuildId == user.GuildId && s.UserId == user.Id);
            }
        }

        public static void AddVoiceMuteToDatabase(ulong userId, ulong guildId)
        {
            var _vRecord = new VoiceMuteRecord()
            {
                GuildId = guildId,
                UserId = userId
            };

            Database.UpsertRecord(_vRecord);
        }

        public static void RemoveVoiceMuteFromDatabase(ulong userId, ulong guildId)
        {
            var _vRecord = Database.GetRecord<VoiceMuteRecord>(s => s.UserId == userId && s.GuildId == guildId);

            if (_vRecord != null)
            {
                Database.DeleteRecord<VoiceMuteRecord>(s => s.UserId == userId && s.GuildId == guildId);
            }
        }

        public static async Task VoiceMuteUser(IGuildUser user)
        {
            await user.ModifyAsync(s => s.Mute = true);
        }

        public static async Task VoiceUnmuteUser(IGuildUser user)
        {
            await user.ModifyAsync(s => s.Mute = false);
        }

        public static async Task<IRole> GetOrCreateMutedRoleAsync(IGuild guild)
        {
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == guild.Id);

            // Create muted role if it doesn't exist
            if (_config.MutedRoleID == 0 || guild.GetRole(_config.MutedRoleID) == null)
            {
                if (guild.Roles.Any(s => s.Name == "muted" || s.Name == "Muted"))
                {
                    var ___role = guild.Roles.FirstOrDefault(s => s.Name == "muted" || s.Name == "Muted");
                    _config.MutedRoleID = ___role.Id;
                    return ___role;
                }

                LogService.Log.Warning($"Guild \"{guild.Name}\" does not have a muted role. Creating one now..");

                var __role = await guild.CreateRoleAsync("Muted", GuildPermissions.None, Color.DarkerGrey);
                
                // Set role permissions
                foreach (var channel in await guild.GetTextChannelsAsync())
                {
                    var ___perms = new OverwritePermissions(sendMessages: PermValue.Deny);
                    await channel.AddPermissionOverwriteAsync(__role, ___perms);
                }

                // Add to database
                _config.MutedRoleID = __role.Id;
                Database.UpsertRecord(_config);

                return __role;
            }

            return guild.GetRole(_config.MutedRoleID);
        }

        public static async Task UnmuteUserFromID(ulong guildId, ulong userId)
        {
            var _guild = Program.client.GetGuild(guildId);
            var _user = _guild.GetUser(userId);

            if (_user == null || _guild == null) return;

            var _role = await GetOrCreateMutedRoleAsync(_guild);

            await _user.RemoveRoleAsync(_role);

            Database.DeleteRecord<MuteRecord>(s => s.GuildId == guildId && s.UserId == userId);

            LogService.Log.Information($"User with ID \"{userId}\" has been unmuted from timed ban.");
        }
    }
}
