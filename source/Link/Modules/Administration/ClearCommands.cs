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
        public ClearService ClearService { get; set; }

        [Command("clear")]
        [Alias("prune")]
        [Summary("Clears a flat amount of messages in a channel.")]
        public async Task ClearCommand(int count)
            => await ClearService.ClearCommandService(Context, count).ConfigureAwait(false);

        [Command("clear")]
        [Alias("prune")]
        [Summary("Clears an amount of messages from a certain user.")]
        public async Task ClearCommand(IUser user, int count)
            => await ClearService.ClearCommandService(Context, user, count).ConfigureAwait(false);
    }
}
