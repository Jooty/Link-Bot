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
        [Summary("Join a channel.")]
        public async Task JoinCommand()
        {
            AudioService.JoinAudio(Context);
        }

        [Command("play")]
        [Summary("Join a channel, then play a song.")]
        public async Task PlayCommand([Remainder]string song)
        {
            AudioService.Play(Context, song);
        }

        [Command("skip")]
        [Summary("Skips the current song. Requires DJ.")]
        [RequireDJ]
        public async Task SkipCommand()
        {
            AudioService.Skip(Context);
        }

        [Command("pause")]
        [Summary("Pauses the current song. Requires DJ.")]
        [RequireDJ]
        public async Task PauseCommand()
        {
            AudioService.Pause(Context);
        }

        [Command("resume")]
        [Alias("unpause")]
        [Summary("Resumes the current song. Requires DJ.")]
        [RequireDJ]
        public async Task ResumeCommand()
        {
            AudioService.Resume(Context);
        }

        [Command("remove")]
        [Summary("Removes a song at index. Requires DJ.")]
        [RequireDJ]
        public async Task RemoveCommand(int index)
        {
            AudioService.Remove(Context, index);
        }
    }
}
