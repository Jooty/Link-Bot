using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link
{
    public class RequireHierarchyAttribute : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            ParameterInfo parameter, object value, IServiceProvider services)
        {
            // Hierarchy is only available under the socket variant of the user.
            if (!(context.User is SocketGuildUser guildUser))
                return PreconditionResult.FromError("");

            SocketGuildUser targetUser;
            switch (value)
            {
                case SocketGuildUser targetGuildUser:
                    targetUser = targetGuildUser;
                    break;
                case ulong userId:
                    targetUser = await context.Guild.GetUserAsync(userId).ConfigureAwait(false) as SocketGuildUser;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (targetUser == null)
                return PreconditionResult.FromError("Target user not found.");

            if (guildUser.Hierarchy < targetUser.Hierarchy)
                return PreconditionResult.FromError("");

            var currentUser = await context.Guild.GetCurrentUserAsync().ConfigureAwait(false) as SocketGuildUser;
            if (currentUser?.Hierarchy < targetUser.Hierarchy)
                return PreconditionResult.FromError("");

            return PreconditionResult.FromSuccess();
        }
    }
}
