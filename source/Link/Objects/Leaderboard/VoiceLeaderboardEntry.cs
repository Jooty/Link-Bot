using System;
using LiteDB;

namespace Link
{
    public class VoiceLeaderboardEntry
    {
        public class Stats
        {
            public ulong UserId  { get; set; }
            public ulong GuildId { get; set; }
        }

        [BsonId]    public Stats    EntryStats      { get; set; }
        [BsonField] public TimeSpan TotalTime       { get; set; }
        [BsonField] public TimeSpan TimeAwake       { get; set; }
        [BsonField] public TimeSpan TimeMuted       { get; set; }
        [BsonField] public TimeSpan TimeDeafened    { get; set; }
        [BsonField] public TimeSpan TimeServerMuted { get; set; }
    }
}
