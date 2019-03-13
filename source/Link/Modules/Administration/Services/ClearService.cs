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
    public class ClearService
    {
        // The time in seconds it takes for me to start recording message deleting.
        // This is to prevent me from logging my own deletes.
        private static int timeToWait = 1;
        public static List<ulong> CurrentClears = new List<ulong>();

        public static async Task ClearCommandService(SocketCommandContext Context, int count)
        {
            if (count > 100)
            {
                await Respond.SendResponse(Context, $"Use a number less than 100.");
                return;
            }

            AddToTimedList(Context.Channel.Id);

            var _msgs = Context.Channel.GetCachedMessages(count + 1);
            await Context.Channel.DeleteMessagesAsync(_msgs);
        }

        public static async Task ClearCommandService(SocketCommandContext Context, IUser user, int count)
        {
            if (count > 100)
            {
                await Respond.SendResponse(Context, $"Use a number less than 100.");
                return;
            }

            AddToTimedList(Context.Channel.Id);

            var _msgs = Context.Channel.GetCachedMessages(100).Where(s => s.Author.Id == user.Id);
            await Context.Channel.DeleteMessagesAsync(_msgs);
        }

        public static async Task ClearCommandService(SocketCommandContext Context, int count, IUser user)
        {
            if (count > 100)
            {
                await Respond.SendResponse(Context, $"Use a number less than 100.");
                return;
            }

            AddToTimedList(Context.Channel.Id);

            var _msgs = Context.Channel.GetCachedMessages(100).Where(s => s.Author.Id == user.Id);
            await Context.Channel.DeleteMessagesAsync(_msgs);
        }

        public static async Task ClearCommandService(SocketCommandContext Context, ulong userId, int count)
        {
            if (count > 100)
            {
                await Respond.SendResponse(Context, $"Use a number less than 100.");
                return;
            }

            AddToTimedList(Context.Channel.Id);

            var _msgs = Context.Channel.GetCachedMessages(100).Where(s => s.Author.Id == userId);
            await Context.Channel.DeleteMessagesAsync(_msgs);
        }

        public static async Task ClearCommandService(SocketCommandContext Context, int count, ulong userId)
        {
            if (count > 100)
            {
                await Respond.SendResponse(Context, $"Use a number less than 100.");
                return;
            }

            AddToTimedList(Context.Channel.Id);

            var _msgs = Context.Channel.GetCachedMessages(100).Where(s => s.Author.Id == userId);
            await Context.Channel.DeleteMessagesAsync(_msgs);
        }

        private static async Task AddToTimedList(ulong channelId)
        {
            CurrentClears.Add(channelId);

            await Task.Delay(TimeSpan.FromSeconds(timeToWait));

            CurrentClears.Remove(channelId);
        }
    }
}
