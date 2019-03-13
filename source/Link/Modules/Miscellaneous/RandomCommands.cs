﻿using System;
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
        [Command("roll")]
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
        public async Task FlipCoinCommand()
        {
            var _r = new Random().Next(0, 1);

            await Respond.SendResponse(Context, $"{(_r == 0 ? "Heads" : "Tails")}.");
        }

        [Command("8ball")]
        public async Task Magic8Command([Remainder]string question = "")
            => MiscService.Magic8Service(Context);

        [Command("invite")]
        public async Task InviteCommand()
        {
            await Respond.SendResponse(Context, "https://discordapp.com/api/oauth2/authorize?client_id=401994184602681344&permissions=8&scope=bot");
        }
    }
}
