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
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser _user = context.User as IGuildUser;
            if (_user == null)
            {
                return PreconditionResult.FromError("");
            }

            if (Program.Config.DeveloperIDs.Any(s => s == _user.Id))
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("You must be a developer to use this command.");
            }
        }
    }
}
