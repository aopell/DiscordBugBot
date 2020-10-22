using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBugBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DiscordBugBot.Data;

namespace DiscordBugBot.Commands.Modules
{
    [Group("category")]
    public class CategoryModule : ModuleBase<SocketCommandContext>
    {
        public BugBotDataContext DataStore { get; set; }

        [ModeratorRequired]
        [Command("create")]
        [Summary("Create an issue category")]
        public async Task Create(string name, string prefix, string emoji)
        {
            if (await ((IQueryable<IssueCategory>)DataStore.Categories).AnyAsync(c => c.GuildId == Context.Guild.Id && c.Name == name))
            {
                throw new CommandExecutionException($"A category with the name `{name}` already exists in this server");
            }

            IssueCategory category = new IssueCategory
            {
                Name = name,
                Prefix = prefix.ToUpper(),
                EmojiIcon = emoji,
                Archived = false,
                GuildId = Context.Guild.Id
            };

            DataStore.Add(category);
            DataStore.SaveChanges();

            await Context.Channel.SendMessageAsync($"Category `{category.Name}` created. The first issue in this category will be {category.EmojiIcon} `{category.Prefix}-{category.Issues.Count + 1}`");
        }

        [VoterRequired]
        [Command("list")]
        [Summary("List issue categories")]
        public async Task List()
        {
            var categories = ((IQueryable<IssueCategory>)DataStore.Categories).Where(c => c.GuildId == Context.Guild.Id);
            string message = string.Join(
                "\n",
                categories.Select(
                    c => (c.Archived ? "~~" : "") +
                         $"{c.EmojiIcon} **{c.Name}**: Next issue is **{c.Prefix}-{c.Issues.Count + 1}**" +
                         (c.Archived ? "~~ *(archived)*" : "")
                )
            );

            await Context.Channel.SendMessageAsync("**__Available Categories__**\n" + (message.Length > 0 ? message : "*no categories*"));
        }
    }
}
