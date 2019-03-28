using System;
using LiteDB;

namespace Link
{
    public class GameLeaderboardEntry
    {
        public class Stats
        {
            public ulong  UserId { get; set; }
            public string Game   { get; set; }
        }
        [BsonId]    public Stats    EntryStats { get; set; }
        [BsonField] public TimeSpan Time       { get; set; }
    }
}
