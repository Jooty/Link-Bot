using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Link
{
    public class TagService
    {
        public static async Task SendTag(SocketCommandContext Context, string tag)
        {
            tag = tag.ToLower();

            var _tag = Database.GetRecord<Tag>(s => s.Stats.Alias == tag && s.Stats.GuildId == Context.Guild.Id);
            if (_tag == null)
            {
                await Respond.SendResponse(Context, $"Tag not found.");
                return;
            }

            _tag.TimesUsed++;
            Database.UpsertRecord(_tag);

            await Context.Channel.SendMessageAsync(_tag.Message);
        }

        public static async Task CreateTag(SocketCommandContext Context, string tag, string message)
        {
            tag = tag.ToLower();

            // Check for keywords
            List<string> _keywords = new List<string>()
            {
                "create",
                "add",
                "delete",
                "edit",
            };
            if (_keywords.Any(s => s.ToLower() == tag))
            {
                await Respond.SendResponse(Context, "That alias is not available, as it's a keyword.");
                return;
            }

            // Check if tag already exists
            if (Database.GetRecord<Tag>(s => s.Stats.Alias == tag && s.Stats.GuildId == Context.Guild.Id) != null)
            {
                await Respond.SendResponse(Context, "A tag already exists for that alias.");
                return;
            }

            var _newTag = new Tag()
            {
                Stats = new Tag.TagStats()
                {
                    GuildId = Context.Guild.Id,
                    Alias = tag
                },
                Message = message,
                CreatedBy = $"{Context.User.Username}#{Context.User.Discriminator}",
                CreatedAt = DateTime.UtcNow
            };

            Database.UpsertRecord(_newTag);

            await Respond.SendResponse(Context, "Tag created.");
        }

        public static async Task EditTag(SocketCommandContext Context, string tag, string newMessage)
        {
            var _tag = Database.GetRecord<Tag>(s => s.Stats.Alias == tag);
            if (_tag == null)
            {
                await Respond.SendResponse(Context, "That tag does not exist.");
                return;
            }

            if (_tag.CreatedBy != $"{Context.User.Username}#{Context.User.Discriminator}"
                || !(Context.User as IGuildUser).GuildPermissions.Administrator)
            {
                await Respond.SendResponse(Context, "You do not have the required permissions to edit this tag.");
                return;
            }

            _tag.Message = newMessage;

            Database.UpsertRecord(_tag);

            await Respond.SendResponse(Context, "Tag changed successfully.");
        }

        public static async Task DeleteTag(SocketCommandContext Context, string tag)
        {
            tag = tag.ToLower();

            var _tag = Database.GetRecord<Tag>(s => s.Stats.Alias == tag);
            if (_tag == null)
            {
                await Respond.SendResponse(Context, "That tag does not exist.");
                return;
            }

            if (_tag.CreatedBy != $"{Context.User.Username}#{Context.User.Discriminator}"
                 || !(Context.User as IGuildUser).GuildPermissions.Administrator)
            {
                await Respond.SendResponse(Context, "You do not have the required permissions to edit this tag.");
                return;
            }

            Database.DeleteRecord<Tag>(s => s.Stats.Alias == tag && s.Stats.GuildId == Context.Guild.Id);

            await Respond.SendResponse(Context, "Tag deleted successfully.");
        }

        public static async Task TagStats(SocketCommandContext Context, string tag)
        {
            tag = tag.ToLower();

            var _tag = Database.GetRecord<Tag>(s => s.Stats.Alias == tag);
            if (_tag == null)
            {
                await Respond.SendResponse(Context, "That tag does not exist.");
                return;
            }

            var _embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle(tag)
                .AddField("Created by:", _tag.CreatedBy, true)
                .AddField("Created on:", _tag.CreatedAt.ToLongDateString(), true)
                .AddField("Times used:", _tag.TimesUsed)
                .WithDescription($"**Returns**:\n{_tag.Message}");

            await Context.Channel.SendMessageAsync(embed: _embed.Build());
        }
    }
}
