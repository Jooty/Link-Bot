using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeSearch;

namespace Link
{
    public class AudioQueueEntry
    {
        public VideoInformation VideoInformation { get; set; }
        public IUser User { get; set; }

        public AudioQueueEntry(VideoInformation info, IUser user)
        {
            VideoInformation = info;
            User = user;
        }
    }
}
