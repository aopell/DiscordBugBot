using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBugBot.Commands;
using DiscordBugBot.Data;
using DiscordBugBot.Models;
using DiscordBugBot.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBugBot.Helpers
{
    public class IssueHelper
    {
        private static readonly Regex IssueMentionPattern = new Regex(@"`([A-Z]+-\d+)`");

        private BugBotDataContext dataStore;
        private DiscordBot mainInstance;

        public IssueHelper(BugBotDataContext dataStore, DiscordBot mainInstance)
        {
            this.dataStore = dataStore;
            this.mainInstance = mainInstance;
        }

        public async Task CreateIssue(Proposal proposal, ISocketMessageChannel channel, IUserMessage message, GuildOptions options, string title = null, string description = null)
        {
            var category = proposal.Category ?? await dataStore.Categories.FindAsync(proposal.CategoryId);
            int number = category.NextNumber;
            category.NextNumber++;
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
                Priority = IssuePriority.Low,
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
                var logchannel = (ISocketMessageChannel)mainInstance.Client.GetChannel(options.LoggingChannelId.Value);
                var logmessage = await logchannel.SendMessageAsync(embed: GenerateLogIssueEmbed(issue, category));
                _ = logmessage.AddReactionsAsync(IssueLogHelper.IssueLogReactions);
                issue.LogMessageId = logmessage.Id;
            }

            dataStore.Add(issue);
            await dataStore.SaveChangesAsync();

            //await channel.SendMessageAsync("Approved!", embed: IssueEmbedHelper.GenerateInlineIssueEmbed(issue, options, category));
        }

        public async Task<(IssueCategory, bool)> UpdateIssue(Issue issue, IssueUpdateArgs args)
        {
            string number = issue.Number;
            IssueCategory category = null;
            issue.Title = args.Title ?? issue.Title;
            issue.Description = args.Description ?? issue.Description;

            if (ReferenceEquals(args.Assignee, IssueUpdateArgs.NoUser))
            {
                issue.Assignee = null;
            }
            else
            {
                issue.Assignee = args.Assignee?.Id ?? issue.Assignee;
            }

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
                category = dataStore.Categories.SingleOrDefault(c => c.GuildId == issue.GuildId && c.Name == args.Category);
                if (category is null) throw new CommandExecutionException("Invalid value for `Category`");

                string newNumber = $"{category.Prefix}-{category.NextNumber}";

                category.NextNumber++;
                await dataStore.SaveChangesAsync();

                issue.CategoryId = category.Id;
                issue.Number = newNumber;
            }

            await dataStore.SaveChangesAsync();

#warning Unawaited task
            _ = UpdateLogIssueEmbed(issue, issue.Category);

            return (category, number != issue.Number);
        }

        public async Task ProcessMessageTextForInlineMention(IMessage message)
        {
            if (message.Author.IsBot || !(message.Channel is IGuildChannel gc)) return;

            var matches = IssueMentionPattern.Matches(message.Content);

            if (matches.Count == 0) return;

            foreach (Match match in matches)
            {
                string issueNum = match.Groups[1].Value;
                var issue = await dataStore.Issues.SingleOrDefaultAsync(i => i.GuildId == gc.GuildId && i.Number == issueNum);
                if (issue is null) continue;

                await message.Channel.SendMessageAsync(embed: GenerateInlineIssueEmbed(issue));
            }
        }

        public Embed GenerateInlineIssueEmbed(Issue issue, GuildOptions options = null, IssueCategory category = null)
        {
            options ??= dataStore.GuildOptions.Find(issue.GuildId);
            return GenerateIssueEmbed(issue, category, options, true);
        }

        public Embed GenerateLogIssueEmbed(Issue issue, IssueCategory category = null)
        {
            return GenerateIssueEmbed(issue, category, null, false);
        }

        private Embed GenerateIssueEmbed(Issue issue, IssueCategory category, GuildOptions options, bool inline)
        {
            category ??= issue.Category ?? dataStore.Categories.Single(c => c.Id == issue.CategoryId);

            var embed = new EmbedBuilder()
                        .WithTitle($"{category?.EmojiIcon} `{issue.Number}` {issue.Title}")
                        .WithDescription(issue.Description)
                        .WithTimestamp(issue.LastUpdatedTimestamp)
                        .WithColor(Strings.StatusColors[issue.Status])
                        .WithImageUrl(issue.ImageUrl)
                        .WithThumbnailUrl(issue.ThumbnailUrl)
                        .WithFields(
                            new EmbedFieldBuilder
                            {
                                Name = "Status",
                                Value = $"{Strings.StatusEmojis[issue.Status]} {issue.Status}",
                                IsInline = true
                            },
                            new EmbedFieldBuilder
                            {
                                Name = "Priority",
                                Value = $"{Strings.PriorityEmojis[issue.Priority]} {issue.Priority}",
                                IsInline = true
                            },
                            new EmbedFieldBuilder
                            {
                                Name = "Author",
                                Value = mainInstance.Client.GetUser(issue.Author).ToString(),
                                IsInline = true
                            },
                            new EmbedFieldBuilder
                            {
                                Name = $"Jump to {(inline ? "Issue Log" : "Message")}",
                                Value = $"[Click Here](https://canary.discordapp.com/channels/{issue.GuildId}/{(inline ? options.LoggingChannelId : issue.ChannelId)}/{(inline ? issue.LogMessageId : issue.MessageId)})",
                                IsInline = true
                            }
                        )
                        .WithFooter($"Category: {issue.Category.Name} • Last Updated");

            if (issue.Assignee.HasValue)
            {
                IUser assignee = mainInstance.Client.GetUser(issue.Assignee.Value);
                EmbedAuthorBuilder author = new EmbedAuthorBuilder
                {
                    Name = $"{assignee.Username}#{assignee.Discriminator}",
                    IconUrl = assignee.GetAvatarUrl() ?? assignee.GetDefaultAvatarUrl()
                };

                embed.Author = author;
            }
            else
            {
                embed.Author = new EmbedAuthorBuilder
                {
                    Name = "Nobody assigned"
                };
            }

            return embed.Build();
        }


        public async Task UpdateLogIssueEmbed(Issue issue, IssueCategory category = null, GuildOptions options = null)
        {
            options ??= dataStore.GuildOptions.Find(issue.GuildId);
            if (options?.LoggingChannelId is null) return;

            if (!(mainInstance.Client.GetChannel(options.LoggingChannelId.Value) is IMessageChannel logChannel)) return;

            if (!(await logChannel.GetMessageAsync(issue.LogMessageId) is IUserMessage logMessage)) return;

            Embed embed = GenerateLogIssueEmbed(issue, category);
            await logMessage.ModifyAsync(mp => mp.Embed = embed);
        }

        public async Task HandleLogMessageReaction(IMessageChannel channel, IReaction reaction, IUser reactionUser, IUserMessage message)
        {
            if (reactionUser is null || reactionUser.IsBot) return;
            if (!(channel is IGuildChannel gc)) return;
            var issue = await dataStore.Issues.SingleOrDefaultAsync(i => i.GuildId == gc.GuildId && i.LogMessageId == message.Id);
            if (issue is null) return;
            _ = message.RemoveReactionAsync(reaction.Emote, reactionUser);
            var options = dataStore.GuildOptions.Find(gc.GuildId);
            if (options is null) return;
            (_, bool mod) = IssueConfirmationHelper.GetVoterStatus((IGuildUser)reactionUser, options);
            if (!mod) return;
            var action = IssueLogHelper.IssueLogReactionActions.GetValueOrDefault(reaction.Emote.ToString());
            if (action is null) return;
            await action(this, issue, reactionUser);
        }
    }
}
