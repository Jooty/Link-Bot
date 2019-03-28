using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Link
{
    public class LinkBot
    {
        public static DiscordSocketClient client;
        public static BotConfig Config;

        public static void Main(string[] args)
            => Init().GetAwaiter().GetResult();

        public static async Task Init()
        {
            Config = BotConfigService.GetConfig();

            LogService.Initialize();

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 10000,
                AlwaysDownloadUsers = true,
                DefaultRetryMode = RetryMode.AlwaysRetry
            });

            SetupEvents();

            // Begin invoke of database mute timers
            Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    .Subscribe(x => MuteService.UpdateDatabaseTimers());
                    #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await client.LoginAsync(TokenType.Bot, Config.Token);
            await client.StartAsync();

            await SetBotStatuses().ConfigureAwait(false);

            await Task.Delay(-1);
        }

        private static void SetupEvents()
        {
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
        }

        private static async Task SetBotStatuses()
        {
            var _config = BotConfigService.GetConfig();

            #region Status

            int _defaultStatus = 1;
            if (_config.DefaultStatus < 1 || _config.DefaultStatus > 4)
            {
                // Goto default
                await client.SetStatusAsync(UserStatus.Online);
                LogService.Log.Warning("Default Status must be a value through 1 and 4 in the Config.json!");

                _defaultStatus = 1;
            }

            switch (_defaultStatus)
            {
                case 1:
                    await client.SetStatusAsync(UserStatus.Online);
                    break;
                case 2:
                    await client.SetStatusAsync(UserStatus.Idle);
                    break;
                case 3:
                    await client.SetStatusAsync(UserStatus.DoNotDisturb);
                    break;
                case 4:
                    await client.SetStatusAsync(UserStatus.Offline);
                    break;
            }
            #endregion

            #region Activity

            string[] _values = _config.DefaultActivity.Split('|');

            if (int.TryParse(_values[0], out var _activityIndex))
            {
                switch (_activityIndex)
                {
                    case 1:
                        await client.SetGameAsync(_values[1], type: ActivityType.Playing);
                        break;
                    case 2:
                        await client.SetGameAsync(_values[1], type: ActivityType.Listening);
                        break;
                    case 3:
                        await client.SetGameAsync(_values[1], type: ActivityType.Watching);
                        break;
                    case 4:
                        await client.SetGameAsync(_values[1], type: ActivityType.Streaming);
                        break;
                }
            }
            else
            {
                LogService.Log.Warning("Could not parse DefaultActivity successfully! Exmample: \"3|My Testing Realm..\"");

                return;
            }

            #endregion
        }
    }
}
