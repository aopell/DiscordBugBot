using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBugBot.Models;
using DiscordBugBot.Tools;
using System.Linq;

namespace DiscordBugBot.Helpers
{
    public static class IssueEmbedHelper
    {
        public static Embed GenerateInlineIssueEmbed(Issue issue, GuildOptions options = null, IssueCategory category = null)
        {
            options ??= DiscordBot.MainInstance.DataStore.GuildOptions.Find(issue.GuildId);
            return GenerateIssueEmbed(issue, category, options, true);
        }

        public static Embed GenerateLogIssueEmbed(Issue issue, IssueCategory category = null)
        {
            return GenerateIssueEmbed(issue, category, null, false);
        }

        private static Embed GenerateIssueEmbed(Issue issue, IssueCategory category, GuildOptions options, bool inline)
        {
            category ??= issue.Category ?? DiscordBot.MainInstance.DataStore.Categories.Single(c => c.Id == issue.CategoryId);

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
                                Value = DiscordBot.MainInstance.Client.GetUser(issue.Author).ToString(),
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
                IUser assignee = DiscordBot.MainInstance.Client.GetUser(issue.Assignee.Value);
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
    }
}
