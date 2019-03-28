using LiteDB;

namespace Link
{
    public class VoiceMuteRecord
    {
        [BsonId]    public ulong UserId  { get; set; }
        [BsonField] public ulong GuildId { get; set; }
    }
}
