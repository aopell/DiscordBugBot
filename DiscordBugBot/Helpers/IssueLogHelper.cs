using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBugBot.Models;
using DiscordBugBot.Tools;

namespace DiscordBugBot.Helpers
{
    public delegate Task IssueLogAction(Issue issue, IUser user);

    public static class IssueLogHelper
    {
        public static IEmote[] IssueLogReactions => IssueLogReactionActions.Keys.Select(s => Emote.TryParse(s, out Emote e) ? (IEmote)e : new Emoji(s)).ToArray();

        private static readonly Dictionary<string, IssueLogAction> IssueLogReactionActions = new Dictionary<string, IssueLogAction>
        {
            [Strings.OrangeUpArrowEmoji] = IncreasePriority,
            [Strings.BlueDownArrowEmoji] = DecreasePriority,
            [Strings.OrangeStarEmoji] = MarkToDo,
            [Strings.BlueDotEmoji] = MarkInProgress,
            [Strings.GreenCheckEmoji] = MarkDone,
            [Strings.InboxEmoji] = ClaimIssue,
            [Strings.OutboxEmoji] = UnclaimIssue,
            [Strings.GithubEmoji] = EscalateToGithub
        };

        public static async Task UpdateLogIssueEmbed(Issue issue, IssueCategory category = null, GuildOptions options = null)
        {
            options ??= DiscordBot.MainInstance.DataStore.GetOptions(issue.GuildId);
            if (options?.LoggingChannelId is null) return;

            if (!(DiscordBot.MainInstance.Client.GetChannel(options.LoggingChannelId.Value) is IMessageChannel logChannel)) return;

            if (!(await logChannel.GetMessageAsync(issue.LogMessageId) is IUserMessage logMessage)) return;

            Embed embed = IssueEmbedHelper.GenerateLogIssueEmbed(issue, category);
            await logMessage.ModifyAsync(mp => mp.Embed = embed);
        }

        public static async Task HandleLogMessageReaction(IMessageChannel channel, IReaction reaction, IUser reactionUser, IUserMessage message)
        {
            if (reactionUser is null || reactionUser.IsBot) return;
            if (!(channel is IGuildChannel gc)) return;
            var issue = DiscordBot.MainInstance.DataStore.GetIssueByLogMessage(gc.GuildId, message.Id);
            if (issue is null) return;
            _ = message.RemoveReactionAsync(reaction.Emote, reactionUser);
            var options = DiscordBot.MainInstance.DataStore.GetOptions(gc.GuildId);
            if (options is null) return;
            IssueConfirmationHelper.GetVoterStatus((IGuildUser)reactionUser, options, out bool mod);
            if (!mod) return;
            var action = IssueLogReactionActions.GetValueOrDefault(reaction.Emote.ToString());
            if (action is null) return;
            await action(issue, reactionUser);
        }

        private static async Task EscalateToGithub(Issue issue, IUser user)
        {
            await user.SendMessageAsync("That feature isn't implemented yet");
        }

        private static async Task UnclaimIssue(Issue issue, IUser user)
        {
            if (issue.Assignee != user.Id) return;
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Assignee = IssueUpdateArgs.NoUser
                }
            );
        }

        private static async Task ClaimIssue(Issue issue, IUser user)
        {
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Assignee = user
                }
            );
        }


        private static async Task MarkDone(Issue issue, IUser user)
        {
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Status = IssueStatus.Done.ToString()
                }
            );
        }

        private static async Task MarkInProgress(Issue issue, IUser user)
        {
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Status = IssueStatus.InProgress.ToString()
                }
            );
        }

        private static async Task MarkToDo(Issue issue, IUser user)
        {
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Status = IssueStatus.ToDo.ToString()
                }
            );
        }

        private static async Task DecreasePriority(Issue issue, IUser user)
        {
            if (issue.Priority == IssuePriority.Low) return;
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Priority = (issue.Priority - 1).ToString()
                }
            );
        }

        private static async Task IncreasePriority(Issue issue, IUser user)
        {
            if (issue.Priority == IssuePriority.VeryHigh) return;
            await IssueModificationHelper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Priority = (issue.Priority + 1).ToString()
                }
            );
        }
    }
}
