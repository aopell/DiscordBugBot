using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBugBot.Models;
using System.Linq;

namespace DiscordBugBot.Commands.Modules
{
    [Group("category")]
    public class CategoryModule : ModuleBase<BotCommandContext>
    {
        [ModeratorRequired]
        [Command("create")]
        [Summary("Create an issue category")]
        public async Task Create(string name, string prefix, string emoji)
        {
            if (Context.Bot.DataStore.GetCategory(Context.Guild.Id, name) != null)
            {
                throw new CommandExecutionException($"A category with the name `{name}` already exists in this server");
            }

            IssueCategory category = new IssueCategory
            {
                Name = name,
                Prefix = prefix.ToUpper(),
                EmojiIcon = emoji,
                Archived = false,
                GuildId = Context.Guild.Id,
                NextNumber = 1
            };

            Context.Bot.DataStore.CreateCategory(category);

            await Context.Channel.SendMessageAsync($"Category `{category.Name}` created. The first issue in this category will be {category.EmojiIcon} `{category.Prefix}-{category.NextNumber}`");
        }

        [VoterRequired]
        [Command("list")]
        [Summary("List issue categories")]
        public async Task List()
        {
            var categories = Context.Bot.DataStore.GetCategories(Context.Guild.Id);
            string message = string.Join(
                "\n",
                categories.Select(
                    c => (c.Archived ? "~~" : "") +
                         $"{c.EmojiIcon} **{c.Name}**: Next issue is **{c.Prefix}-{c.NextNumber}**" +
                         (c.Archived ? "~~ *(archived)*" : "")
                )
            );

            await Context.Channel.SendMessageAsync("**__Available Categories__**\n" + (message.Length > 0 ? message : "*no categories*"));
        }
    }
}
