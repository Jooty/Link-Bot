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
    public class ChangeNameService
    {
        public static async Task ChangeName(SocketCommandContext Context, string name)
        {
            await Respond.SendResponse(Context, $"I am about to change my name to **{name}**. Are you sure you want to continue? (yes/no)");

            var _answer = ConfirmationService.instance.AwaitSimpleReply(Context);

            if (_answer)
            {
                await Context.Client.CurrentUser.ModifyAsync(s => s.Username = name);

                await Respond.SendResponse(Context, $"Changed name to **{name}**.");
            }
            else
            {
                await Respond.SendResponse(Context, $"Cancelled.");
            }
        }
    }
}
