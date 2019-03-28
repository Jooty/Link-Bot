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
    public class MuteCommands : ModuleBase<SocketCommandContext>
    {
        public MuteService MuteService { get; set; }

        [Command("Mute")]
        [Summary("Mutes a user indefinetly.")]
        public async Task MuteCommand(IGuildUser user)
            => await MuteService.Mute(Context, user).ConfigureAwait(false);

        [Command("Mute")]
        [Summary("Mutes a user for a timed amount. Use the following format: `1m30` or `1h30m`.")]
        public async Task MuteCommand(IGuildUser user, string time)
            => await MuteService.Mute(Context, user, time).ConfigureAwait(false);

        [Command("Unmute")]
        [Summary("Unmutes a user.")]
        public async Task UnmuteCommand(IGuildUser user)
            => await MuteService.Unmute(Context, user).ConfigureAwait(false);
    }
}
