using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBugBot.Helpers;
using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands.Modules
{
    [Group("issue")]
    public class IssueModule : ModuleBase<BotCommandContext>
    {
        [Command]
        [Summary("Gets an issue")]
        public async Task Get(string number)
        {
            Issue issue = Context.Bot.DataStore.GetIssueByNumber(Context.Guild.Id, number);
            if (issue is null)
            {
                throw new CommandExecutionException("That issue does not exist");
            }
            await Context.Channel.SendMessageAsync(embed: IssueEmbedHelper.GenerateInlineIssueEmbed(issue));
        }

        [Command("create")]
        [Summary("Create an issue")]
        public async Task Create(string category, string title, [Remainder] string description)
        {
            Proposal proposal = new Proposal
            {
                Category = category,
                ChannelId = Context.Channel.Id,
                GuildId = Context.Guild.Id,
                MessageId = Context.Message.Id,
                Status = ProposalStatus.Approved
            };

            GuildOptions options = Context.Bot.DataStore.GetOptions(Context.Guild.Id);

            if (options is null)
            {
                throw new CommandExecutionException("Please set up the server before trying to create issues. Run the `setup` command to get started.");
            }

            Context.Bot.DataStore.CreateProposal(proposal);
            await IssueConfirmationHelper.CreateIssue(proposal, Context.Channel, Context.Message, options, title: title, description: description);
        }

        [NamedArgumentType]
        public class UpdateArgs
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public IUser Assignee { get; set; }
            public string Status { get; set; }
            public string Priority { get; set; }
            public string Category { get; set; }
            public string Image { get; set; }
            public string Thumbnail { get; set; }
        }

        [Command("update")]
        [Summary("Updates one or more fields on an issue")]
        public async Task Update(string number, UpdateArgs args)
        {
            Issue issue = Context.Bot.DataStore.GetIssueByNumber(Context.Guild.Id, number);
            IssueCategory category = null;
            if (issue is null)
            {
                throw new CommandExecutionException("That issue does not exist");
            }

            issue.Title = args.Title ?? issue.Title;
            issue.Description = args.Description ?? issue.Description;
            issue.Assignee = args.Assignee?.Id ?? issue.Assignee;
            issue.ImageUrl = args.Image ?? issue.ImageUrl;
            issue.ThumbnailUrl = args.Thumbnail ?? issue.ThumbnailUrl;

            if (args.Status != null && Enum.TryParse(typeof(IssueStatus), args.Status.Replace(" ", ""), true, out object s))
            {
                issue.Status = s as IssueStatus? ?? throw new CommandExecutionException("Invalid value for `Status`");
            }
            else if (args.Status != null)
            {
                throw new CommandExecutionException("Invalid value for `Status`");
            }

            if (args.Priority != null && Enum.TryParse(typeof(IssuePriority), args.Priority.Replace(" ", ""), true, out object p))
            {
                issue.Priority = p as IssuePriority? ?? throw new CommandExecutionException("Invalid value for `Priority`");
            }
            else if (args.Priority != null)
            {
                throw new CommandExecutionException("Invalid value for `Priority`");
            }

            if (args.Category != null)
            {
                category = Context.Bot.DataStore.GetCategory(Context.Guild.Id, args.Category);
                if (category is null) throw new CommandExecutionException("Invalid value for `Category`");

                string newNumber = $"{category.Prefix}-{category.NextNumber}";

                category.NextNumber++;
                Context.Bot.DataStore.UpdateCategory(category);

                issue.Category = args.Category;
                issue.Number = newNumber;

                await Context.Channel.SendMessageAsync($":warning: **Issue `{number}` has been renumbered as `{newNumber}`**");
            }

            category ??= Context.Bot.DataStore.GetCategory(Context.Guild.Id, issue.Category);

            Context.Bot.DataStore.UpdateIssue(issue);
            await Context.Channel.SendMessageAsync(embed: IssueEmbedHelper.GenerateInlineIssueEmbed(issue, category: category));
            await IssueEmbedHelper.UpdateLogIssueEmbed(issue, category);
        }
    }
}
