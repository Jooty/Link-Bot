using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Link
{
    public class RandomCommands : ModuleBase<SocketCommandContext>
    {
        public MiscService MiscService { get; set; }

        [Command("ping")]
        [Summary("Pings the bot.")]
        public async Task PingCommand() 
            => await Respond.SendResponse(Context, $"Pong! `{Context.Client.Latency}ms`");

        [Command("roll")]
        [Summary("Roll a dice in D&D style. Example: `1d20`")]
        public async Task RollCommand(string query)
        {
            string[] _params = query.Split('d');

            List<int> _rollValues = new List<int>();

            Random _r = new Random();
            for (int i = 0; i < Int32.Parse(_params[0]); i++)
            {
                _rollValues.Add(_r.Next(1, Int32.Parse(_params[1])));
            }

            if (!(_rollValues.Count > 5))
            {
                StringBuilder _builder = new StringBuilder();
                foreach (var value in _rollValues)
                {
                    _builder.Append($"{Environment.NewLine}Roll {_rollValues.IndexOf(value) + 1}: {_rollValues[_rollValues.IndexOf(value)]}");
                }

                await ReplyAsync($"Rolled `{_params[0]}` time(s). ```{_builder}```\n**Total: {_rollValues.Sum()}**");
            }
            else
            {
                await ReplyAsync($"Rolled `{_params[0]}` time(s).\n**Total: {_rollValues.Sum()}**");
            }
        }

        [Command("coin")]
        [Alias("flipacoin", "coin", "flipcoin", "coinflip")]
        [Summary("Flips a coin.")]
        public async Task FlipCoinCommand()
        {
            var _r = new Random().Next(0, 1);

            await Respond.SendResponse(Context, $"{(_r == 0 ? "Heads" : "Tails")}.");
        }

        [Command("random")]
        public async Task RandomCommand(int one, int two)
        {
            await Respond.SendResponse(Context, new Random().Next(one, two).ToString());
        }

        [Command("pfp")]
        public async Task PFPCommand(IUser user)
        {
            await ReplyAsync("", false, new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithImageUrl(user.GetAvatarUrl()).Build());
        }

        [Command("8ball")]
        [Summary("Ask the 8-ball a question.")]
        public async Task Magic8Command([Remainder]string question = "")
            => await MiscService.Magic8Service(Context).ConfigureAwait(false);

        [Command("invite")]
        [Summary("Returns an invite to invite me anywhere.")]
        public async Task InviteCommand()
        {
            await Respond.SendResponse(Context, "https://discordapp.com/api/oauth2/authorize?client_id=401994184602681344&permissions=8&scope=bot");
        }
    }
}
