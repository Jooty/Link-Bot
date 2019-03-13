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
    [RequireUserPermission(GuildPermission.KickMembers)]
    [Group]
    public class KickCommands : ModuleBase<SocketCommandContext>
    {
        [Command("kick")]
        [Summary("Kicks a user from the guild.")]
        public async Task KickCommand([RequireHierarchy]IUser user, string reason = "")
        {
            // Create reason
            if (reason == "")
                reason = $"{Context.User.Username}#{Context.User.Discriminator} kicked this user. No reason provided.";
            else
                reason = $"{Context.User.Username}#{Context.User.Discriminator} kicked this user for: {reason}";

            await (user as IGuildUser).KickAsync(reason);

            if (reason == "")
                await Respond.SendResponse(Context, $"Kicked user **{user.Username}#{user.Discriminator}**.");
            else
                await Respond.SendResponse(Context, $"Kicked user **{user.Username}#{user.Discriminator}** for `{reason}`.");
        }
    }
}
