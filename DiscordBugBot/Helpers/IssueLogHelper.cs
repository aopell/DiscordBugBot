using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBugBot.Models;

namespace DiscordBugBot.Helpers
{
    public static class IssueLogHelper
    {
        public static async Task UpdateLogIssueEmbed(Issue issue, IssueCategory category = null, GuildOptions options = null)
        {
            options ??= DiscordBot.MainInstance.DataStore.GetOptions(issue.GuildId);
            if (options?.LoggingChannelId is null) return;

            if (!(DiscordBot.MainInstance.Client.GetChannel(options.LoggingChannelId.Value) is IMessageChannel logChannel)) return;

            if (!(await logChannel.GetMessageAsync(issue.LogMessageId) is IUserMessage logMessage)) return;

            Embed embed = IssueEmbedHelper.GenerateLogIssueEmbed(issue, category);
            await logMessage.ModifyAsync(mp => mp.Embed = embed);
        }

        public static async Task HandleLogMessageReaction(IMessageChannel channel, IReaction reaction, IUserMessage message)
        {

        }
    }
}
