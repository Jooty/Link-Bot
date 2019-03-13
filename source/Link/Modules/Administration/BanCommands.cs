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
    [RequireUserPermission(GuildPermission.BanMembers)]
    [Group]
    public class BanCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        public async Task BanCommand([RequireHierarchy]IUser user, string reason = "")
        {
            // Create reason
            if (reason == "")
                reason = $"{Context.User.Username}#{Context.User.Discriminator} banned this user. No reason provided.";
            else
                reason = $"{Context.User.Username}#{Context.User.Discriminator} banned this user for: {reason}";

            await Context.Guild.AddBanAsync(user, reason: reason);

            if (reason == "")
                await Respond.SendResponse(Context, $"Banned user **{user.Username}#{user.Discriminator}**.");
            else
                await Respond.SendResponse(Context, $"Banned user **{user.Username}#{user.Discriminator}** for reason: `{reason}`.");
        }

        [Command("forceban")]
        public async Task ForcebanCommand(ulong id)
            => BanService.ForcebanUser(Context, id);

        [Command("pardon")]
        [Alias("unban")]
        public async Task PardonCommand(ulong id)
            => BanService.PardonUser(Context, id);
    }
}
