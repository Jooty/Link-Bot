using System;
using LiteDB;

namespace Link
{
    public class Tag
    {
        public class TagStats
        {
            public ulong  GuildId { get; set; }
            public string Alias   { get; set; }
        }
        [BsonId]    public TagStats Stats     { get; set; }
        [BsonField] public string Message     { get; set; }
        [BsonField] public string CreatedBy   { get; set; }
        [BsonField] public DateTime CreatedAt { get; set; }
        [BsonField] public int TimesUsed      { get; set; }
    }
}
