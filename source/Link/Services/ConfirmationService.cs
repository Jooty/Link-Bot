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
    public class ConfirmationService
    {
        public static ConfirmationService instance;
        private readonly DiscordSocketClient client;

        private class ConfirmEntry
        {
            public ConfirmEntry(IUser user) { User = user; }
            public IUser User { get; set; }
            public bool HasReplied = false;
            public string Reply = "";
        }
        private List<ConfirmEntry> awaitingReplies = new List<ConfirmEntry>();

        public ConfirmationService()
        {
            client = Program.client;
            instance = this;

            client.MessageReceived += Client_MessageReceived;
        }

        public bool AwaitSimpleReply(SocketCommandContext Context)
        {
            var _entry = new ConfirmEntry(Context.User);
            awaitingReplies.Add(_entry);

            while (!_entry.HasReplied)
            {
                continue;
            }

            if (_entry.Reply == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task Client_MessageReceived(SocketMessage msg)
        {
            if (msg.Author.IsBot) return;

            // Check to see if I need their reply
            if (awaitingReplies.Any(s => s.User == msg.Author))
            {
                var _entry = awaitingReplies.First(s => s.User == msg.Author);

                if (msg.Content == "yes" || msg.Content == "Yes")
                {
                    _entry.HasReplied = true;
                    _entry.Reply = "yes";

                    awaitingReplies.Remove(_entry);
                }
                else if (msg.Content == "no" || msg.Content == "No")
                {
                    _entry.HasReplied = true;
                    _entry.Reply = "no";

                    awaitingReplies.Remove(_entry);
                }
                else
                {
                    awaitingReplies.Remove(_entry);
                    return;
                }
            }
        }
    }
}
