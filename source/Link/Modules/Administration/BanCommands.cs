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
        [Summary("Ban a user by mention or ID.")]
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
        [Summary("Bans a user by ID. Works even when they're not in the server.")]
        public async Task ForcebanCommand(ulong id)
            => BanService.ForcebanUser(Context, id);

        [Command("pardon")]
        [Alias("unban")]
        [Summary("Pardons a user.")]
        public async Task PardonCommand(ulong id)
            => BanService.PardonUser(Context, id);
    }
}
