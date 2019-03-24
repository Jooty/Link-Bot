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
    [Group]
    public class VoiceLeaderboardCommands : ModuleBase<SocketCommandContext>
    {
        [Command("vlb")]
        [Alias("voiceleaderboard")]
        [Summary("Gets the leaderboard for the most hours in a voice channel for this guild.")]
        public async Task VoiceLeaderboardCommand()
        {
            var _leaderboard = LeaderboardService.GetVoiceLeaderboard(Context.Guild).OrderByDescending(s => s.TotalTime).ToList();

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle($"Voice leaderboard for: {Context.Guild.Name} ({Context.Guild.Users.Count})");

            StringBuilder _builder = new StringBuilder();
            foreach (var user in _leaderboard)
            {
                var _user = Context.Guild.GetUser(user.EntryStats.UserId);

                _builder.Append($"\n" +
                    $"`{_leaderboard.IndexOf(user) + 1}.)` " +
                    $"**{_user.Mention ?? (_user.Username + "#" + _user.Discriminator)}** " +
                    $"- Total Time: `{Math.Round(user.TotalTime.TotalHours, 2)}` hours");
            }
            _embed.Description = _builder.ToString();

            await ReplyAsync(embed: _embed.Build());
        }

        [Command("level")]
        [Alias("vcl", "voicelevel", "voice")]
        [Summary("Gets your stats for voice in this guild.")]
        public async Task VoiceLevelCommand(IGuildUser user = null)
        {
            var _user = user ?? Context.User as IGuildUser;
            var _userRec = LeaderboardService.GetVoiceHours(Context.Guild.Id, _user.Id);

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle($"Hours in voice: \"{_user.Username}\" for \"{Context.Guild.Name}\"")
                .WithDescription($"Total: `{Math.Round(_userRec.TotalTime.TotalHours), 2}` hours" +
                    $"\nTime Awake: `{Math.Round(_userRec.TimeAwake.TotalHours), 2}` hours" +
                    $"\nTime Muted: `{Math.Round(_userRec.TimeMuted.TotalHours), 2}` hours" +
                    $"\nTime Server Muted: `{Math.Round(_userRec.TimeServerMuted.TotalHours), 2}` hours" +
                    $"\nTime Deafened: `{Math.Round(_userRec.TimeDeafened.TotalHours), 2}` hours");

            await ReplyAsync(embed: _embed.Build());
        }
    }
}
