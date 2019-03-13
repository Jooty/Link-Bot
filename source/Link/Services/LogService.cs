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
                Console.WriteLine($"Could not find guild configuration for guild: \"{guildId}\" when it was requested.");
                return null;
            }

            if (!_record.Log || _record.LogChannelID == 0) return null;
            else return Program.client.GetChannel(_record.LogChannelID) as ITextChannel;
        }
    }
}
