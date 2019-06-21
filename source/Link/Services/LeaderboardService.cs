using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;

namespace Link
{
    public class LeaderboardService
    {
        public LeaderboardService()
        {
            // Begin invoke
            Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(x => UpdateAllUsersVoice());

            Observable
                    .Interval(TimeSpan.FromSeconds(1))
                    .Subscribe(x => UpdateAllUsersGame());
        }

        public static VoiceLeaderboardEntry[] GetVoiceLeaderboard(IGuild guild) 
            => Database.GetRecords<VoiceLeaderboardEntry>(s => s.EntryStats.GuildId == guild.Id)
            .Where(s => s.TotalTime.TotalMilliseconds > 0)
            .OrderBy(s => s.TotalTime)
            .ToArray();

        public static VoiceLeaderboardEntry GetVoiceHours(ulong guildId, ulong userId)
            => Database.GetRecord<VoiceLeaderboardEntry>(s => s.EntryStats.GuildId == guildId && s.EntryStats.UserId == userId);

        public static GameLeaderboardEntry[] GetGameLeaderboard(IGuild guild)
        {
            return Database.GetRecords<GameLeaderboardEntry>(s => s.Time.TotalMilliseconds != 0).ToArray()
                .Where(s => guild.GetUsersAsync().Result.Any(x => x.Id == s.EntryStats.UserId))
                .Where(s => s.Time.TotalMilliseconds > 0)
                .ToArray();
        }

        public static GameLeaderboardEntry[] GetGameLeaderboard(string game) 
            => Database.GetRecords<GameLeaderboardEntry>(s => s.EntryStats.Game == game)
            .OrderBy(s => s.Time)
            .ToArray();

        public static GameLeaderboardEntry GetGameHours(ulong guildId, ulong userId, string game)
            => Database.GetRecord<GameLeaderboardEntry>(s => s.EntryStats.UserId == userId && s.EntryStats.Game == game);

        public static GameLeaderboardEntry[] GetPersonalGameLeaderboard(ulong userId)
            => Database.GetRecords<GameLeaderboardEntry>(s => s.EntryStats.UserId == userId);

        private void UpdateAllUsersVoice()
        {
            List<ulong> _seenUsers = new List<ulong>();

            foreach (var guild in LinkBot.client.Guilds)
            {
                foreach (var user in guild.Users)
                {
                    if (user.IsBot 
                        || _seenUsers.Contains(user.Id) 
                        || user.VoiceChannel == null
                        || user.VoiceChannel == guild.AFKChannel)
                    {
                        continue;
                    }

                    var _curRecord = Database.GetRecord<VoiceLeaderboardEntry>(s => s.EntryStats.UserId == user.Id && s.EntryStats.GuildId == guild.Id);
                    if (_curRecord == null)
                    {
                        VoiceLeaderboardEntry _newEntry = new VoiceLeaderboardEntry()
                        {
                            EntryStats = new VoiceLeaderboardEntry.Stats()
                            {
                                UserId = user.Id,
                                GuildId = guild.Id
                            },
                            TotalTime = new TimeSpan(),
                            TimeAwake = new TimeSpan(),
                            TimeMuted = new TimeSpan(),
                            TimeDeafened = new TimeSpan(),
                            TimeServerMuted = new TimeSpan()
                        };

                        Database.UpsertRecord(_newEntry);
                        _curRecord = _newEntry;
                    }

                    _curRecord.TotalTime = _curRecord.TotalTime.Add(TimeSpan.FromSeconds(1));

                    if (user.IsSelfMuted)
                    {
                        _curRecord.TimeMuted = _curRecord.TimeMuted.Add(TimeSpan.FromSeconds(1));
                    }
                    else if (user.IsSelfDeafened)
                    {
                        _curRecord.TimeDeafened = _curRecord.TimeDeafened.Add(TimeSpan.FromSeconds(1));
                    }
                    else if (user.IsMuted)
                    {
                        _curRecord.TimeServerMuted = _curRecord.TimeServerMuted.Add(TimeSpan.FromSeconds(1));
                    }
                    else
                    {
                        _curRecord.TimeAwake = _curRecord.TotalTime.Add(TimeSpan.FromSeconds(1));
                    }

                    Database.UpsertRecord(_curRecord);
                    _seenUsers.Add(user.Id);
                }
            }
        }

        private void UpdateAllUsersGame()
        {
            List<ulong> _updatedUsers = new List<ulong>();

            foreach (var guild in LinkBot.client.Guilds)
            {
                foreach (var user in guild.Users)
                {
                    if (_updatedUsers.Contains(user.Id) || user.Activity == null || user.IsBot)
                    {
                        continue;
                    }

                    var _curRecord = Database.GetRecord<GameLeaderboardEntry>(s =>
                        s.EntryStats.UserId == user.Id
                        && s.EntryStats.Game == user.Activity.Name);

                    if (_curRecord == null)
                    {
                        Database.UpsertRecord(new GameLeaderboardEntry()
                        {
                            EntryStats = new GameLeaderboardEntry.Stats()
                            {
                                UserId = user.Id,
                                Game = user.Activity.Name
                            },
                            Time = new TimeSpan()
                        });

                        _curRecord = Database.GetRecord<GameLeaderboardEntry>(s =>
                            s.EntryStats.UserId == user.Id
                            && s.EntryStats.Game == user.Activity.Name);
                    }

                    _curRecord.Time = _curRecord.Time.Add(TimeSpan.FromSeconds(1));
                    Database.UpsertRecord(_curRecord);

                    _updatedUsers.Add(user.Id);
                }
            }
        }
    }
}
