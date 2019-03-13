using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link
{
    public class MuteRecord
    {
        public ulong GuildId { get; set; }
        [LiteDB.BsonId]
        public ulong UserId { get; set; }
        public TimeSpan ?Time { get; set; }
    }
}
