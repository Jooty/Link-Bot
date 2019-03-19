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
    public class RequireDJAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var _roleId = await GetOrCreateDJRoleIDAsync(context);
            if (_roleId == 0)
            {
                return PreconditionResult.FromError("The DJ role could not be found. Can you create one for me? I'll find it automatically.");
            }

            if ((context.User as IGuildUser).RoleIds.Any(id => id == _roleId))
            {
                return PreconditionResult.FromError("Only a DJ can perform this command!");
            }
            else if ((context.User as IGuildUser).GuildPermissions.MoveMembers)
            {
                return PreconditionResult.FromSuccess();
            }
            else
            {
                return PreconditionResult.FromSuccess();
            }
        }

        public static async Task<ulong> GetOrCreateDJRoleIDAsync(ICommandContext Context)
        {
            var _config = Database.GetRecord<GuildConfig>(s => s.ID == Context.Guild.Id);
            if (_config == null)
            {
                LogService.Log.Warning($"Could not find guild configuration for guild ID: \"{Context.Guild}\" when it was requested.");
                return 0;
            }

            if (_config.DJRoleID == 0)
            {
                // Try to find one, if it exists
                if (Context.Guild.Roles.Any(s => s.Name == "DJ"))
                {
                    var _role = Context.Guild.Roles.First(s => s.Name == "DJ");

                    _config.DJRoleID = _role.Id;

                    return _role.Id;
                }

                var _newRole = await Context.Guild.CreateRoleAsync("DJ");

                _config.DJRoleID = _newRole.Id;

                return _newRole.Id;
            }
            else
            {
                var _role = Context.Guild.GetRole(_config.DJRoleID);

                if (_role == null)
                {
                    var _newRole = await Context.Guild.CreateRoleAsync("DJ");

                    _config.DJRoleID = _newRole.Id;

                    return _newRole.Id;
                }
                else
                {
                    return _role.Id;
                }
            }
        }
    }
}
