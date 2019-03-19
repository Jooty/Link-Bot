using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Newtonsoft.Json;
using PastebinAPI;

namespace Link
{
    [RequireDeveloper]
    public class DeveloperCommands
    {
        public DeveloperService DevService { get; set; }

        [Group("Database")]
        public class DeveloperDatabaseCommands : ModuleBase<SocketCommandContext>
        {
            [Group("upsert")]
            public class CreateCommands : ModuleBase<SocketCommandContext>
            {
                [Command("guildconfig")]
                public async Task CreateGuildConfig(ulong guildID = 0)
                {
                    var _guild = guildID == 0
                        ? Context.Guild
                        : Context.Client.GetGuild(guildID);

                    if (_guild == null)
                    {
                        await ReplyAsync($"I am not connected to any guild with the ID: `{guildID}`.");
                        return;
                    }

                    Database.CreateDefaultGuildConfig(Context.Guild);

                    await ReplyAsync($"Guild config created for guild with ID: `{_guild.Id}`");
                }
            }

            [Group("delete")]
            public class DeleteCommands : ModuleBase<SocketCommandContext>
            {
                [Command("guildconfig")]
                public async Task DeleteGuildConfig(ulong id = 0)
                {
                    if (id == 0)
                    {
                        id = Context.Guild.Id;
                    }
                    else if (id != 0)
                    {
                        var _guild = Context.Client.GetGuild(id);

                        if (_guild == null)
                        {
                            await ReplyAsync($"I am not connected to any guild with the ID: `{id}`.");
                            return;
                        }
                    }

                    // Find if exists
                    var _col = Database.GetRecords<GuildConfig>(s => s.ID == Context.Guild.Id);
                    if (_col == null || _col.Count() == 0)
                    {
                        await ReplyAsync($"I do not have any guild configuration records for ID: `{id}`.");
                        return;
                    }

                    Database.DeleteRecord<GuildConfig>(s => s.ID == id);
                    await ReplyAsync($"Successfully deleted guild configuration record for guild ID: `{id}`");
                }
            }

            [Group("get")]
            public class GetCommands : ModuleBase<SocketCommandContext>
            {
                [Command("mute")]
                public async Task GetMuteRecordCommand(ulong userId, ulong guildId = 0)
                {
                    // Get corresponding guild
                    if (guildId == 0)
                    {
                        guildId = Context.Guild.Id;
                    }
                    else
                    {
                        var __guild = Context.Client.GetGuild(guildId);
                        if (__guild == null)
                        {
                            await Respond.SendResponse(Context, $"Could not find guild with ID: `{guildId}`");
                            return;
                        }
                    }

                    var _record = Database.GetRecord<MuteRecord>(s => s.UserId == userId && s.GuildId == guildId);
                    await Respond.SendResponse(Context, (!(_record == null)).ToString());
                }

                [Command("guildconfig")]
                public async Task GetGuildConfigCommand(ulong guildID = 0)
                {
                    if (guildID == 0)
                        guildID = Context.Guild.Id;

                    if (Context.Client.GetGuild(guildID) == null)
                        await ReplyAsync($"I am not connected to any guild with the ID: `{guildID}`.");

                    var _record = Database.GetRecord<GuildConfig>(s => s.ID == guildID);
                    if (_record == null)
                    {
                        await ReplyAsync($"I do not have a record for the guild ID: `{guildID}`");
                        return;
                    }

                    StringBuilder _forcebans = new StringBuilder();
                    if (_record.Forcebans.Count != 0)
                    {
                        foreach (var ban in _record.Forcebans)
                        {
                            _forcebans.Append($"\n{ban}");
                        }
                    }
                    else
                    {
                        _forcebans.Append("null");
                    }

                    var _embed = new EmbedBuilder()
                        .WithTitle($"Guild config for: `{guildID}`")
                        .WithThumbnailUrl(Context.Client.GetGuild(guildID).IconUrl)
                        .WithColor(Color.Blue)
                        .WithDescription($"Log enabled: {_record.Log}" +
                                         $"\nLog Channel ID: {_record.LogChannelID}" +
                                         $"\nName: {_record.Name}" +
                                         $"\nOwner: {_record.OwnerName}" +
                                         $"\nOwner ID: {_record.OwnerID}")
                        .AddField("Forcebans", _forcebans.ToString());
                    await ReplyAsync("", false, _embed.Build());
                }
            }

            [Group("reset")]
            public class ResetCommands : ModuleBase<SocketCommandContext>
            {
                [Command("guildconfig")]
                public async Task ResetGuildConfigCommand()
                {
                    foreach (var guild in Context.Client.Guilds)
                    {
                        Database.DeleteRecord<GuildConfig>(s => s.ID == guild.Id);

                        Database.CreateDefaultGuildConfig(Context.Guild);
                    }

                    await Respond.SendResponse(Context, "Restored all guild configurations to default.");
                }
            }
        }

        [Group]
        public class DeveloperBasicCommands : ModuleBase<SocketCommandContext>
        {
            [Command("shutdown")]
            public async Task ShutdownCommand()
            {
                await Respond.SendResponse(Context, ":thumbsup:");

                Environment.Exit(1);
            }

            [Command("restart")]
            public async Task RestardCommand()
            {
                await Respond.SendResponse(Context, ":thumbsup:");

                Process.Start(Assembly.GetExecutingAssembly().Location);

                Environment.Exit(1);
            }

            [Command("changename")]
            public async Task ChangeNameCommand([Remainder]string name)
                => DeveloperService.ChangeName(Context, name);

            [Command("botlog")]
            public async Task BotlogCommand(int count = 5)
            {
                var _logs = Database.GetRecords<LogRecord>(s => s.Type == LogType.bot);

                StringBuilder _builder = new StringBuilder();
                foreach (var log in _logs)
                {
                    _builder.Append($"\n[{log.Date}] {log.Log}");
                }

                var _embed = new EmbedBuilder()
                    .WithTitle($"Last {count} bot logs:")
                    .WithColor(Color.Blue)
                    .WithDescription($"```{_builder.ToString()}```");

                await ReplyAsync("", false, _embed.Build());
            }
        }
    }
}
