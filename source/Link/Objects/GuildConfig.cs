using System.Collections.Generic;
using LiteDB;

namespace Link
{
    public class GuildConfig
    {
        [BsonId]    public ulong  ID           { get; set; }
        [BsonField] public string Name         { get; set; }
        [BsonField] public ulong  OwnerID      { get; set; }
        [BsonField] public string OwnerName    { get; set; }
        [BsonField] public ulong  LogChannelID { get; set; }
        [BsonField] public bool   Log          { get; set; }
        [BsonField] public ulong  MutedRoleID  { get; set; }
        [BsonField] public ulong  DJRoleID     { get; set; }

        [BsonField] public List<ulong> Forcebans { get; set; }
    }
}
