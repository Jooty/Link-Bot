using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;

namespace Link
{
    public class RequireDeveloperAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser _user = context.User as IGuildUser;
            if (_user == null)
            {
                return PreconditionResult.FromError("");
            }

            if (GetDeveloperIDs().Any(s => s == _user.Id))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("You must be a developer to use this command.");
            }
        }

        private static List<ulong> GetDeveloperIDs()
        {
            var _lines = File.ReadAllLines($"{Directory.GetCurrentDirectory()}/Resources/Developers.txt");

            var _parsed = new List<ulong>();
            foreach (var line in _lines)
            {
                ulong number;
                bool __success = UInt64.TryParse(line, out number);

                if (__success)
                    _parsed.Add(number);
                else
                    continue;
            }

            return _parsed;
        }
    }
}
