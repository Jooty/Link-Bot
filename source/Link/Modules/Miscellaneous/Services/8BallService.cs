using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.IO;

namespace Link
{
    public class MiscService
    {
        public static async Task Magic8Service(SocketCommandContext Context)
        {
            var _answers = Get8BallAnswers();
            var _r = new Random().Next(0, _answers.Length);

            await Respond.SendResponse(Context, _answers[_r]);
        }

        private static string[] Get8BallAnswers()
        {
            return File.ReadAllLines($"{Directory.GetCurrentDirectory()}/Resources/Magic8BallAnswers.txt");
        }
    }
}
