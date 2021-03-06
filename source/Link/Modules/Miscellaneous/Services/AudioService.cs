﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

namespace Link
{
    public class AudioService
    {
        private static readonly List<AudioClientWrapper> Clients = new List<AudioClientWrapper>();

        public static async Task JoinAudio(SocketCommandContext Context)
        {
            // Already in voice channel
            if (Clients.Any(s => s.GuildId == Context.Guild.Id))
            {
                return;
            }

            // Not in a voice channel
            if ((Context.User as IVoiceState).VoiceChannel == null)
            {
                await Respond.SendResponse(Context, "You are not in a voice channel!");
                return;
            }

            Clients.Add(new AudioClientWrapper(Context));
        }

        public static async Task Play(SocketCommandContext Context, string song)
        {
            if ((Context.User as IVoiceState).VoiceChannel == null)
            {
                await Respond.SendResponse(Context, "You are not in a voice channel!");
                return;
            }

            var _client = Clients.FirstOrDefault(s => s.GuildId == Context.Guild.Id);

            if (_client == null)
            {
                var _new = new AudioClientWrapper(Context);

                await _new.AddToQueue(Context, song);

                Clients.Add(_new);
            }
            else
            {
                await _client.AddToQueue(Context, song);
            }

            await Context.Message.DeleteAsync();
        }

        public static async Task Skip(SocketCommandContext Context)
        {
            var _client = Clients.FirstOrDefault(s => s.GuildId == Context.Guild.Id);

            if (_client == null)
            {
                await Respond.SendResponse(Context, "No player is connected to this guild.");
                return;
            }

            _client.Skip();
        }

        public static async Task Pause(SocketCommandContext Context)
        {
            if (!Clients.Any(s => s.GuildId == Context.Guild.Id))
            {
                await Respond.SendResponse(Context, "No player is connected to this guild.");
                return;
            }

            await Clients.FirstOrDefault(s => s.GuildId == Context.Guild.Id).Pause();
        }

        public static async Task Resume(SocketCommandContext Context)
        {
            if (!Clients.Any(s => s.GuildId == Context.Guild.Id))
            {
                await Respond.SendResponse(Context, "No player is connected to this guild.");
                return;
            }

            await Clients.FirstOrDefault(s => s.GuildId == Context.Guild.Id).Resume();
        }

        public static async Task Remove(SocketCommandContext Context, int index)
        {
            if (!Clients.Any(s => s.GuildId == Context.Guild.Id))
            {
                await Respond.SendResponse(Context, "No player is connected to this guild.");
                return;
            }

            await Clients.FirstOrDefault(s => s.GuildId == Context.Guild.Id).Remove(Context, index);
        }
    }
}
