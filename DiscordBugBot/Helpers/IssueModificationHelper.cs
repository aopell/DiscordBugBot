using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBugBot.Commands;
using DiscordBugBot.Data;
using DiscordBugBot.Models;

namespace DiscordBugBot.Helpers
{
    public static class IssueModificationHelper
    {
        public static IDataStore DataStore => DiscordBot.MainInstance.DataStore;
        public static DiscordSocketClient Client => DiscordBot.MainInstance.Client;

        public static async Task CreateIssue(Proposal proposal, ISocketMessageChannel channel, IUserMessage message, GuildOptions options, string title = null, string description = null)
        {
            var category = DataStore.GetCategory(proposal.GuildId, proposal.Category);
            int number = category.NextNumber;
            category.NextNumber++;
            DataStore.UpdateCategory(category);
            var now = DateTimeOffset.Now;

            var issue = new Issue
            {
                GuildId = proposal.GuildId,
                ChannelId = proposal.ChannelId,
                MessageId = proposal.MessageId,
                Category = proposal.Category,
                Status = IssueStatus.ToDo,
                Title = title ?? "",
                Number = $"{category.Prefix}-{number}",
                Priority = IssuePriority.Medium,
                Assignee = null,
                Author = message.Author.Id,
                CreatedTimestamp = now,
                LastUpdatedTimestamp = now,
                Description = description ?? message.Content,
                ThumbnailUrl = null,
                ImageUrl = message.Attachments.FirstOrDefault()?.Url
            };

            if (options.LoggingChannelId.HasValue)
            {
                var logchannel = (ISocketMessageChannel)Client.GetChannel(options.LoggingChannelId.Value);
                var logmessage = await logchannel.SendMessageAsync(embed: IssueEmbedHelper.GenerateLogIssueEmbed(issue, category));
                _ = logmessage.AddReactionsAsync(IssueLogHelper.IssueLogReactions);
                issue.LogMessageId = logmessage.Id;
            }

            DataStore.CreateIssue(issue);

            await channel.SendMessageAsync("Approved!", embed: IssueEmbedHelper.GenerateInlineIssueEmbed(issue, options, category));
        }

        public static async Task<(IssueCategory, bool)> UpdateIssue(Issue issue, IssueUpdateArgs args)
        {
            string number = issue.Number;
            IssueCategory category = null;
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
                category = DataStore.GetCategory(issue.GuildId, args.Category);
                if (category is null) throw new CommandExecutionException("Invalid value for `Category`");

                string newNumber = $"{category.Prefix}-{category.NextNumber}";

                category.NextNumber++;
                DataStore.UpdateCategory(category);

                issue.Category = args.Category;
                issue.Number = newNumber;
            }

            category ??= DataStore.GetCategory(issue.GuildId, issue.Category);

            DataStore.UpdateIssue(issue);

            _ = IssueLogHelper.UpdateLogIssueEmbed(issue, category);

            return (category, number != issue.Number);
        }
    }
}
