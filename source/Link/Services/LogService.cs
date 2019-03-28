using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Serilog;
using Serilog.Core;

namespace Link
{
    public class LogService
    {
        public static Logger Log;

        public static void Initialize()
        {
            Log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }

        public static ITextChannel GetLogChannel(ulong guildId)
        {
            var _record = Database.GetRecord<GuildConfig>(s => s.ID == guildId);

            if (_record == null)
            {
                var _guild = LinkBot.client.GetGuild(guildId);

                Database.CreateDefaultGuildConfig(LinkBot.client.GetGuild(guildId), out _record);
            }

            if (!_record.Log || _record.LogChannelID == 0) return null;
            else return LinkBot.client.GetChannel(_record.LogChannelID) as ITextChannel;
        }
    }
}
