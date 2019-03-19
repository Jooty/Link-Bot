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

        public static async Task SendTimedResponse(SocketCommandContext Context, string response, TimeSpan time)
        {
            try
            {
                var _msg = await Context.Channel.SendMessageAsync($"**{Context.User.Username}** | {response}");

                await Task.Delay(time);

                await _msg.DeleteAsync();
            }
            catch (Exception ex)
            {
                LogService.Log.Warning(ex.Message);
            }
        }
    }
}
