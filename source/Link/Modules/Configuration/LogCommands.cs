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
    [RequireUserPermission(GuildPermission.Administrator)]
    public class LogCommands : ModuleBase<SocketCommandContext>
    {
        [Command("log")]
        [Summary("Enables or disables logging for the current channel.")]
        public async Task LogCommand()
        {
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == Context.Guild.Id);

            _config.LogChannelID = Context.Channel.Id;
            _config.Log = !_config.Log;

            Database.UpsertRecord(_config);

            await Respond.SendResponse(Context, $"Logging has been **{(_config.Log ? "Enabled" : "Disabled")}** for this channel.");
        }
    }
}
