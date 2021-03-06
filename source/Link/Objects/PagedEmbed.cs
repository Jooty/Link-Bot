﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Link
{
    public class PagedEmbed
    {
        public class EmbedPage
        {
            public Dictionary<string, string> Fields = new Dictionary<string, string>();
        }
        // ulong to keep channel ID
        private EmbedPage[] embedPages;
        private int currentPage;

        private Emoji backEmoji = new Emoji("◀");
        private Emoji forwardEmoji = new Emoji("▶");

        private string embedName;

        private readonly IUserMessage myEmbedMessage;
        private readonly IUser controller;

        public PagedEmbed(SocketCommandContext Context, EmbedPage[] pages, string embedTitle = "")
        {
            embedName = embedTitle;
            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle($"{embedTitle} (1/{pages.Length})");

            embedPages = pages;

            foreach (var field in pages[0].Fields)
            {
                _embed.AddField(field.Key, field.Value);
            }

            myEmbedMessage = Context.Channel.SendMessageAsync("", false, _embed.Build()).Result;

            currentPage = 0;

            controller = Context.User;

            SetupReactions();
            SetupEvents();
        }

        private void SetupReactions()
        {
            if (embedPages.Count() == 1)
            {
                return;
            }

            myEmbedMessage.AddReactionAsync(backEmoji);
            myEmbedMessage.AddReactionAsync(forwardEmoji);
        }

        private void SetupEvents()
        {
            if (embedPages.Count() == 1)
            {
                return;
            }

            var _client = LinkBot.client;

            _client.ReactionAdded += ReactionAdded;
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (msg.Value.Id != myEmbedMessage.Id || reaction.User.Value != controller)
            {
                return;
            }

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue);

            if (reaction.Emote.Equals(backEmoji))
            {
                if (currentPage - 1 < 0)
                {
                    return;
                }

                currentPage--;

                foreach (var field in embedPages[currentPage].Fields)
                {
                    _embed.AddField(field.Key, field.Value);
                }

                _embed.Title = $"{embedName} ({currentPage + 1}/{embedPages.Length})";

                await UpdateAsync(_embed.Build());
            }
            else if (reaction.Emote.Equals(forwardEmoji))
            {
                if (currentPage + 1 > embedPages.Length)
                {
                    return;
                }

                currentPage++;

                foreach (var field in embedPages[currentPage].Fields)
                {
                    _embed.AddField(field.Key, field.Value);
                }

                _embed.Title = $"{embedName} ({currentPage + 1}/{embedPages.Length})";

                await UpdateAsync(_embed.Build());
            }
        }

        private async Task UpdateAsync(Embed newEmbed)
        {
            await myEmbedMessage.ModifyAsync(s => s.Embed = newEmbed).ConfigureAwait(false);
        }
    }
}
