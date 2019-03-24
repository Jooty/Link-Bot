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
    [Group("tag")]
    public class TagCommands : ModuleBase<SocketCommandContext>
    {
        public TagService TagService { get; set; }

        [Command()]
        public async Task TagCommand(string tag)
            => await TagService.SendTag(Context, tag).ConfigureAwait(false);

        [Command("create")]
        public async Task TagCreateCommand(string tag, [Remainder]string message)
            => await TagService.CreateTag(Context, tag, message).ConfigureAwait(false);

        [Command("edit")]
        public async Task TagEditCommand(string tag, [Remainder]string newMessage)
            => await TagService.EditTag(Context, tag, newMessage);

        [Command("delete")]
        public async Task TagDeleteCommand(string tag)
            => await TagService.DeleteTag(Context, tag);

        [Command("stats")]
        public async Task TagStatsCommand(string tag)
            => await TagService.TagStats(Context, tag);
    }
}
