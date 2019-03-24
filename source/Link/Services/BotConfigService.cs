﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Link
{
    public class BotConfigService
    {
        public static BotConfig GetConfig()
        {
            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(@"Resources/Config.json"));
        }

        public static void SetPrefix(string newPrefix)
        {
            var _config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(@"Resources/Config.json"));

            _config.Prefix = newPrefix;
        }

        public static string GetPrefix()
        {
            var _config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(@"Resources/Config.json"));

            return _config.Prefix;
        }

        public static void SetDefaultStatus(int newStatus)
        {
            var _config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(@"Resources/Config.json"));

            _config.DefaultStatus = newStatus;

            File.WriteAllText(@"/Resources/Config.json", JsonConvert.SerializeObject(_config));
        }

        public static void SetDefaultActivity(int index, string activity)
        {
            var _config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(@"Resources/Config.json"));

            _config.DefaultActivity = $"{index}|{activity}";

            File.WriteAllText(@"/Resources/Config.json", JsonConvert.SerializeObject(_config));
        }
    }
}
