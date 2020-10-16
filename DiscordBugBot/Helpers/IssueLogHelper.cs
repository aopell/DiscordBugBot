using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBugBot.Data;
using DiscordBugBot.Models;
using DiscordBugBot.Tools;

namespace DiscordBugBot.Helpers
{
    public delegate Task IssueLogAction(IssueHelper helper, Issue issue, IUser user);

    internal static class IssueLogHelper
    {
        public static IEmote[] IssueLogReactions => IssueLogReactionActions.Keys.Select(s => Emote.TryParse(s, out Emote e) ? (IEmote)e : new Emoji(s)).ToArray();

        public static readonly IReadOnlyDictionary<string, IssueLogAction> IssueLogReactionActions = new Dictionary<string, IssueLogAction>
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

        private static async Task EscalateToGithub(IssueHelper helper, Issue issue, IUser user)
        {
            await user.SendMessageAsync("That feature isn't implemented yet");
        }

        private static async Task UnclaimIssue(IssueHelper helper, Issue issue, IUser user)
        {
            if (issue.Assignee != user.Id) return;
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Assignee = IssueUpdateArgs.NoUser
                }
            );
        }

        private static async Task ClaimIssue(IssueHelper helper, Issue issue, IUser user)
        {
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Assignee = user
                }
            );
        }


        private static async Task MarkDone(IssueHelper helper, Issue issue, IUser user)
        {
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Status = IssueStatus.Done.ToString()
                }
            );
        }

        private static async Task MarkInProgress(IssueHelper helper, Issue issue, IUser user)
        {
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Status = IssueStatus.InProgress.ToString()
                }
            );
        }

        private static async Task MarkToDo(IssueHelper helper, Issue issue, IUser user)
        {
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Status = IssueStatus.ToDo.ToString()
                }
            );
        }

        private static async Task DecreasePriority(IssueHelper helper, Issue issue, IUser user)
        {
            if (issue.Priority == IssuePriority.Low) return;
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Priority = (issue.Priority - 1).ToString()
                }
            );
        }

        private static async Task IncreasePriority(IssueHelper helper, Issue issue, IUser user)
        {
            if (issue.Priority == IssuePriority.VeryHigh) return;
            await helper.UpdateIssue(
                issue,
                new IssueUpdateArgs
                {
                    Priority = (issue.Priority + 1).ToString()
                }
            );
        }
    }
}
