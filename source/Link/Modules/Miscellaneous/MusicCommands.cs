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
            => await AudioService.JoinAudio(Context).ConfigureAwait(false);

        [Command("play")]
        [Summary("Join a channel, then play a song.")]
        public async Task PlayCommand([Remainder]string song) 
            => await AudioService.Play(Context, song).ConfigureAwait(false);

        [Command("skip")]
        [Summary("Skips the current song. Requires DJ.")]
        [RequireDJ]
        public async Task SkipCommand() 
            => await AudioService.Skip(Context).ConfigureAwait(false);

        [Command("pause")]
        [Summary("Pauses the current song. Requires DJ.")]
        [RequireDJ]
        public async Task PauseCommand() 
            => await AudioService.Pause(Context).ConfigureAwait(false);

        [Command("resume")]
        [Alias("unpause")]
        [Summary("Resumes the current song. Requires DJ.")]
        [RequireDJ]
        public async Task ResumeCommand() 
            => await AudioService.Resume(Context).ConfigureAwait(false);

        [Command("remove")]
        [Summary("Removes a song at index. Requires DJ.")]
        [RequireDJ]
        public async Task RemoveCommand(int index)
            => await AudioService.Remove(Context, index).ConfigureAwait(false);
    }
}
