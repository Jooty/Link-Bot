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
    [Group]
    [RequireContext(ContextType.Guild)]
    public class MusicCommands : ModuleBase<SocketCommandContext>
    {
        public AudioService AudService { get; set; }

        [Command("join")]
        public async Task JoinCommand()
        {
            AudioService.JoinAudio(Context);
        }

        [Command("play")]
        public async Task PlayCommand([Remainder]string song)
        {
            AudioService.Play(Context, song);
        }

        [Command("skip")]
        [RequireDJ]
        public async Task SkipCommand()
        {
            AudioService.Skip(Context);
        }

        [Command("pause")]
        [RequireDJ]
        public async Task PauseCommand()
        {
            AudioService.Pause(Context);
        }

        [Command("resume")]
        [Alias("unpause")]
        [RequireDJ]
        public async Task ResumeCommand()
        {
            AudioService.Resume(Context);
        }

        [Command("remove")]
        [RequireDJ]
        public async Task RemoveCommand(int index)
        {
            AudioService.Remove(Context, index);
        }
    }
}
