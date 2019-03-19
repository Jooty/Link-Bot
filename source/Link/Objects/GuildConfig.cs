using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Link
{
    public class GuildConfig
    {
        public ulong  ID           { get; set; }
        public string Name         { get; set; }
        public ulong  OwnerID      { get; set; }
        public string OwnerName    { get; set; }
        public ulong  LogChannelID { get; set; }
        public bool   Log          { get; set; }
        public ulong  MutedRoleID  { get; set; }
        public ulong  DJRoleID     { get; set; }

        public List<ulong> Forcebans { get; set; }
    }
}
