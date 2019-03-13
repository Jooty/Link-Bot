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
        [Command("Mute")]
        public async Task MuteCommand(IGuildUser user)
            => MuteService.Mute(Context, user);

        [Command("Mute")]
        public async Task MuteCommand(IGuildUser user, string time)
            => MuteService.Mute(Context, user, time);

        [Command("Unmute")]
        public async Task UnmuteCommand(IGuildUser user)
            => MuteService.Unmute(Context, user);
    }
}
