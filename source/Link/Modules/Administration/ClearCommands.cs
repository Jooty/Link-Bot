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
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [Group]
    public class ClearCommands : ModuleBase<SocketCommandContext>
    {
        [Command("clear")]
        [Alias("prune")]
        public async Task ClearCommand(int count)
            => ClearService.ClearCommandService(Context, count);

        [Command("clear")]
        [Alias("prune")]
        public async Task ClearCommand(IUser user, int count)
            => ClearService.ClearCommandService(Context, user, count);

        [Command("clear")]
        [Alias("prune")]
        public async Task ClearCommand(int count, IUser user)
            => ClearService.ClearCommandService(Context, count, user);

        [Command("clear")]
        [Alias("prune")]
        public async Task ClearCommand(ulong userId, int count)
            => ClearService.ClearCommandService(Context, userId, count);

        [Command("clear")]
        [Alias("prune")]
        public async Task ClearCommand(int count, ulong userId)
            => ClearService.ClearCommandService(Context, count, userId);
    }
}
