using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using DiscordBugBot.Models;
using DiscordBugBot.Tools;

namespace DiscordBugBot.Helpers
{
    public static class IssueEmbedHelper
    {
        public static Embed GenerateIssueEmbed(Issue issue, IssueCategory category = null)
        {
            category ??= DiscordBot.MainInstance.DataStore.GetCategory(issue.GuildId, issue.Category);
            return new EmbedBuilder()
                   .WithTitle($"{Strings.StatusEmojis[issue.Status]} {issue.Number}: {issue.Title}")
                   .WithDescription(issue.Description + $"\n\n[Jump to Message](https://canary.discordapp.com/channels/{issue.GuildId}/{issue.ChannelId}/{issue.MessageId})")
                   .WithAuthor(DiscordBot.MainInstance.Client.GetUser(issue.Author))
                   .WithTimestamp(issue.LastUpdatedTimestamp)
                   .WithFooter($"{category?.EmojiIcon ?? Strings.OrangeStarEmoji} {category?.Name ?? "Issue"} | Last Updated")
                   .Build();
        }
    }
}
