using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;

namespace Link
{
    public class Program
    {
        public static DiscordSocketClient client;

        public static BotConfig Config;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(@"Resources/Config.json"));

            LogService.Initialize();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 10000,
                AlwaysDownloadUsers = true
            });

            // Events
            client.Log += DiscordEventHandler.Log;
            client.MessageDeleted += DiscordEventHandler.MessageDeleted;
            client.MessageUpdated += DiscordEventHandler.MessageUpdated;
            client.UserJoined += DiscordEventHandler.UserJoined;
            client.UserLeft += DiscordEventHandler.UserLeft;
            client.Ready += DiscordEventHandler.ClientReady;
            client.JoinedGuild += DiscordEventHandler.JoinedGuild;
            client.LeftGuild += DiscordEventHandler.LeftGuild;
            client.UserVoiceStateUpdated += DiscordEventHandler.UserVoiceStateUpdated;
            client.GuildMemberUpdated += DiscordEventHandler.GuildMemberUpdated;

            // Begin invoke of database mute timers
            IDisposable databaseMuteTimers =
                Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(x => MuteService.UpdateDatabaseTimers());

            await client.LoginAsync(TokenType.Bot, Config.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
