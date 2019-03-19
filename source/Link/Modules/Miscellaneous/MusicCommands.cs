﻿using System;
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
        [Command("join")]
        public async Task JoinCommand()
        {
            AudioService.JoinAudio(Context);
        }

        [Command("play")]
        public async Task PlayCommand([Remainder]string song)
        {
            AudioService.AddToPlaylist(Context, song);
        }

        [Command("skip")]
        public async Task SkipCommand()
        {
            AudioService.Skip(Context);
        }

        [Command("pause")]
        public async Task PauseCommand()
        {
            AudioService.Pause(Context);
        }

        [Command("resume")]
        [Alias("unpause")]
        public async Task ResumeCommand()
        {
            AudioService.Resume(Context);
        }

        [Command("remove")]
        public async Task RemoveCommand(int index)
        {
            AudioService.Remove(Context, index);
        }
    }
}
