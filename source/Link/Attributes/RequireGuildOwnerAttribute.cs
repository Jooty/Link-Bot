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
    public class RequireGuildOwnerAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser _user = context.User as IGuildUser;
            if (_user == null)
            {
                return PreconditionResult.FromError("");
            }

            if (_user.Id == context.Guild.OwnerId)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromError("Only the owner of this guild can perform that command");
            }
        }
    }
}
