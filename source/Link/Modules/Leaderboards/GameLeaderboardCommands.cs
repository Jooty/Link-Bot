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
    public class GameLeaderboardCommands : ModuleBase<SocketCommandContext>
    {
        [Command("glb")]
        [Alias("gameleaderboard")]
        [Summary("Gets a leaderboard of users with the most hours.")]
        public async Task GameLeaderboardCommand()
        {
            var _leaderboard = LeaderboardService.GetGameLeaderboard(Context.Guild).OrderByDescending(s => s.Time).ToList();
            var _userHighest = _leaderboard
                .Where(s => s.EntryStats.UserId == Context.User.Id)
                .OrderByDescending(s => s.Time)
                .FirstOrDefault();

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"Game leaderboards for: {Context.Guild.Name} ({_leaderboard.Count})");

            // Build leaderboard
            StringBuilder _builder = new StringBuilder();
            int _i = 0;
            foreach (var entry in _leaderboard)
            {
                if (_i == 10) break;

                var _user = Context.Guild.GetUser(entry.EntryStats.UserId);

                _builder.Append(
                    $"\n**{_leaderboard.IndexOf(entry) + 1}.)** " +
                    $"**{_user.Mention ?? (_user.Username + "#" + _user.Discriminator)}**" +
                    $" - *{entry.EntryStats.Game}*" +
                    $" - `{Math.Round(entry.Time.TotalHours, 1)}` hours.");

                _i++;
            }

            _builder.Append($"\n\n**Your highest position**:" +
                $"\n**{_leaderboard.IndexOf(_userHighest) + 1}.)** " +
                $"*{_userHighest.EntryStats.Game}* " +
                $"- `{Math.Round(_userHighest.Time.TotalHours, 1)}` hours.");

            _embed.Description = _builder.ToString();

            await ReplyAsync("", false, _embed.Build());
        }

        [Command("glb")]
        [Alias("gameleaderboard")]
        [Summary("Gets a leaderboard of users with the most hours on a certain game.")]
        public async Task GameLeaderboardCommand([Remainder]string game)
        {
            var _leaderboard = LeaderboardService.GetGameLeaderboard(game).ToList();

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"Game leaderboards for: {Context.Guild.Name} on {game} ({_leaderboard.Count})");

            // Build leaderboard
            StringBuilder _builder = new StringBuilder();
            int _i = 0;
            foreach (var entry in _leaderboard)
            {
                if (_i == 10) break;

                var _user = Context.Guild.GetUser(entry.EntryStats.UserId);

                _builder.Append(
                    $"\n**{_leaderboard.IndexOf(entry) + 1}.)** " +
                    $"**{_user.Mention ?? (_user.Username + "#" + _user.Discriminator)}**" +
                    $" - `{Math.Round(entry.Time.TotalHours, 1)}` hours.");

                _i++;
            }
            _embed.Description = _builder.ToString();

            await ReplyAsync("", false, _embed.Build());
        }

        [Command("hours")]
        [Alias("gamehours")]
        [Summary("Gets your most played games showed through Discord.")]
        public async Task GameHoursCommand()
        {
            var _pLeaderboard = LeaderboardService.GetPersonalGameLeaderboard(Context.User.Id).ToList();

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .WithTitle($"Personal game leaderboard for: {Context.User.Username}#{Context.User.Discriminator} ({_pLeaderboard.Count})");

            // Build personal leaderboard
            StringBuilder _builder = new StringBuilder();
            int _i = 0;
            foreach (var entry in _pLeaderboard)
            {
                if (_i == 10) break;

                _builder.Append(
                    $"\n**{_pLeaderboard.IndexOf(entry) + 1}.)** " +
                    $"**{entry.EntryStats.Game}** " +
                    $"`{Math.Round(entry.Time.TotalHours, 1)}` hours.");

                _i++;
            }
            _embed.Description = _builder.ToString();

            await ReplyAsync("", false, _embed.Build());
        }

        [Command("hours")]
        [Alias("gamehours")]
        [Summary("Gets your hours on a certain game.")]
        public async Task GameHoursCommand([Remainder]string game)
        {
            var _hours = LeaderboardService.GetGameHours(Context.Guild.Id, Context.User.Id, game);
            if (_hours == null)
            {
                await Respond.SendResponse(Context, $"You have no recorded hours for: `{game}`.");
                return;
            }

            await Respond.SendResponse(Context, $"You have `{Math.Round(_hours.Time.TotalHours, 2)}` hours on *{game}*.");
        }
    }
}
