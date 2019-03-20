using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Discord;
using LiteDB;

namespace Link
{
    public class Database
    {
        public static void UpsertRecord<T>(T newRecord)
        {
            try
            {
                using (var db = new LiteDatabase("LinkData.db"))
                {
                    var _col = db.GetCollection<T>(typeof(T).Name);

                    _col.Upsert(newRecord);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static T GetRecord<T>(Expression<Func<T, bool>> func)
        {
            using (var db = new LiteDatabase("LinkData.db"))
            {
                var _col = db.GetCollection<T>(typeof(T).Name);

                return _col.FindOne(func);
            }
        }

        public static IEnumerable<T> GetRecords<T>(Expression<Func<T, bool>> func)
        {
            using (var db = new LiteDatabase("LinkData.db"))
            {
                var _col = db.GetCollection<T>(typeof(T).Name);

                return _col.Find(func);
            }
        }

        public static void DeleteRecord<T>(Expression<Func<T, bool>> func)
        {
            using (var db = new LiteDatabase("LinkData.db"))
            {
                var _col = db.GetCollection<T>(typeof(T).Name);

                _col.Delete(func);
            }
        }

        public static void CheckForAllGuildConfigs()
        {
            // Get all configs
            var _configs = GetRecords<GuildConfig>(s => s.ID > 0);

            foreach (var guild in Program.client.Guilds)
            {
                var _record = _configs.First(s => s.ID == guild.Id);

                // Create record if doesn't exist
                if (_record == null)
                {
                    LogService.Log.Warning($"Guild \"{guild.Name}\"({guild.Id}) does not have a guild configuration. Creating one now..");

                    CreateDefaultGuildConfig(guild, out _);
                }
                else if (_record.Name != guild.Name) // Update name
                {
                    LogService.Log.Warning($"Guild \"{guild.Name}\"({guild.Id}) name updated from \"{_record.Name}\" to \"{guild.Name}\"");

                    _record.Name = guild.Name;
                    UpsertRecord(_record);
                }
                else if (_record.OwnerID != guild.OwnerId) // Update owner
                {
                    LogService.Log.Warning($"Guild \"{guild.Name}\"({guild.Id}) owner updated from \"{_record.OwnerName}\" to \"{guild.Owner.Username}#{guild.Owner.Discriminator}\"");

                    _record.OwnerID = guild.OwnerId;
                    _record.OwnerName = guild.Owner.Username + "#" + guild.Owner.Discriminator;
                }
                else
                    continue;
            }
        }

        public static void CreateDefaultGuildConfig(IGuild guild, out GuildConfig config)
        {
            // Get owner
            var _owner = guild.GetOwnerAsync().Result;

            // Get log channel
            ulong _logChannelID = 0;
            var _logChannel = guild.GetChannelsAsync().Result.FirstOrDefault(s => s.Name == "Log" || s.Name == "log");
            if (_logChannel != null)
                _logChannelID = _logChannel.Id;

            // Get muted role
            ulong _mutedRoleID = 0;
            var _mutedRole = guild.Roles.FirstOrDefault(s => s.Name == "muted" || s.Name == "Muted");
            if (_mutedRole != null)
                _mutedRoleID = _mutedRole.Id;

            // Get DJ role
            ulong _djRoleID = 0;
            var _djRole = guild.Roles.FirstOrDefault(s => s.Name == "DJ");
            if (_djRole != null)
                _djRoleID = _djRole.Id;

            GuildConfig _config = new GuildConfig()
            {
                ID = guild.Id,
                Name = guild.Name,
                OwnerID = guild.OwnerId,
                OwnerName = _owner.Username + "#" + _owner.Discriminator,
                LogChannelID = _logChannelID,
                Log = false,
                MutedRoleID = _mutedRoleID,
                DJRoleID = _djRoleID,
                Forcebans = new List<ulong>()
            };

            UpsertRecord(_config);
            config = _config;

            LogService.Log.Information($"Created guild configuration for guild ID \"{guild.Id}\"");
        }
    }
}