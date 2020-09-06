using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBugBot.Helpers
{
    public static class IssueInlineMentionHelper
    {
        private static readonly Regex IssueMentionPattern = new Regex(@"`([A-Z]+-\d+)`");

        public static async Task ProcessMessageText(SocketMessage message)
        {
            if (message.Author.IsBot || !(message.Channel is IGuildChannel gc)) return;

            var matches = IssueMentionPattern.Matches(message.Content);

            if (matches.Count == 0) return;

            foreach (var match in matches.ToList())
            {
                var issue = DiscordBot.MainInstance.DataStore.GetIssueByNumber(gc.GuildId, match.Groups[1].Value);
                if (issue is null) continue;

                await message.Channel.SendMessageAsync(embed: IssueEmbedHelper.GenerateInlineIssueEmbed(issue));
            }
        }
    }
}
