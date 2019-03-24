﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Link
{
    public class BotConfig
    {
        [JsonProperty]
        public string Token { get; private set; }

        [JsonProperty]
        public string Prefix { get; set; }

        [JsonProperty]
        public int DefaultStatus { get; set; }

        [JsonProperty]
        public string DefaultActivity { get; set; }

        [JsonProperty]
        public List<ulong> DeveloperIDs { get; set; }
    }
}
