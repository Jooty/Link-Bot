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
    public class Respond
    {
        public static async Task SendResponse(SocketCommandContext Context, string response)
        {
            await Context.Channel.SendMessageAsync($"**{Context.User.Username}** | {response}");
        }
    }
}
