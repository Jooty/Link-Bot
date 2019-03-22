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
        public static CommandService commands;
        private static DiscordSocketClient client;
        private static IServiceProvider services;

        public CommandHandlingService(DiscordSocketClient _client) =>
            InitializeAsync(_client).GetAwaiter().GetResult();

        public static IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(commands)
            .AddSingleton<ConfirmationService>()
            .AddSingleton<LeaderboardService>()
            .AddSingleton<BanService>()
            .AddSingleton<DeveloperService>()
            .AddSingleton<ClearService>()
            .AddSingleton<MuteService>()
            .AddSingleton<AudioService>()
            .BuildServiceProvider();

        public async Task InitializeAsync(DiscordSocketClient _client)
        {
            client = _client;
            commands = new CommandService();
            services = BuildServiceProvider();

            await commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: services);
            client.MessageReceived += HandleCommandAsync;

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

            if (!(message.HasStringPrefix(Program.Config.Prefix, ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(client, message);

            var result = await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: services);

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
