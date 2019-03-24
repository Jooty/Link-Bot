using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Link
{
    public class Tag
    {
        public class TagStats
        {
            public ulong GuildId { get; set; }
            public string Alias { get; set; }
        }
        [LiteDB.BsonId]
        public TagStats Stats { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TimesUsed { get; set; }
    }
}
