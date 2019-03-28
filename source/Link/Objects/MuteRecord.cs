using System;
using LiteDB;

namespace Link
{
    public class MuteRecord
    {
        [BsonId]    public ulong    UserId  { get; set; }
        [BsonField] public ulong    GuildId { get; set; }
        [BsonField] public TimeSpan ?Time   { get; set; }
    }
}
