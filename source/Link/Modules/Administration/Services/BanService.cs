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
    public class BanService
    {
        public static async Task ForcebanUser(SocketCommandContext Context, ulong id)
        {
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == Context.Guild.Id);

            _config.Forcebans.Add(id);

            Database.UpsertRecord(_config);

            await Respond.SendResponse(Context, $"User with ID `{id}` has been forcebanned from this guild.");
        }

        public static async Task PardonUser(SocketCommandContext Context, ulong id)
        {
            bool _didPardon = false;

            // Do native unban
            var _bans = await Context.Guild.GetBansAsync();
            if (_bans.Any(s => s.User.Id == id))
            {
                await Context.Guild.RemoveBanAsync(_bans.First(s => s.User.Id == id).User);
                _didPardon = true;
            }

            // Pardon forceban, if one exists
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == Context.Guild.Id);

            if (_config.Forcebans.Any(s => s == id))
            {
                _config.Forcebans.Remove(id);

                Database.UpsertRecord(_config);
                _didPardon = true;
            }

            if (_didPardon)
            {
                await Respond.SendResponse(Context, $"User with ID `{id}` has been pardoned.");
            }
            else
            {
                await Respond.SendResponse(Context, $"User with ID `{id}` has not been banned or forcebanned.");
            }
        }

        public static async Task CheckNewUserForBan(IGuildUser user)
        {
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == user.GuildId);

            var _userRecord = _config.Forcebans.First(s => s == user.Id);
            if (_userRecord == user.Id) await ForcebanNewUser(user);
        }

        public static async Task ForcebanNewUser(IGuildUser user)
            => await user.Guild.AddBanAsync(user, 0, "This user has been forcebanned from this guild.");
    }
}
