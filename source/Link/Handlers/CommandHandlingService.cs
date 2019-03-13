using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Link
{
    public class CommandHandlingService
    {
        private DiscordSocketClient client;
        private CommandService commands;

        public CommandHandlingService(DiscordSocketClient _client) =>
            InitializeAsync(_client).GetAwaiter().GetResult();


        public async Task InitializeAsync(DiscordSocketClient _client)
        {
            client = _client;
            client.MessageReceived += HandleCommandAsync;

            commands = new CommandService();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            foreach (var module in commands.Modules)
            {
                LogService.Log.Debug($"ADDED MODULE: {module.Name}");
            }
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('>', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(client, message);

            var result = await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // LOG
            LogService.Log.Information($"{context.User.Username}#{context.User.Discriminator}: {context.Message.Content}");

            var _record = new LogRecord()
            {
                Date = DateTime.Now,
                Type = LogType.user,
                Log = $"COMMAND[{context.User.Username}{context.User.Discriminator}]: {context.Message}"
            };
            Database.UpsertRecord(_record);

            // TEMP
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync($"`{result.ErrorReason}`");
        }
    }
}
