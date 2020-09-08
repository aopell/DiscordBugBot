using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Tools
{
    public static class Strings
    {
        public const string GreenCheckEmoji = "<:ready:706270984973451295>";
        public const string IndeterminateEmoji = "<:maybe:706702223446376517>";
        public const string RedXEmoji = "<:unavailable:706702240467124345>";
        public const string OrangeStarEmoji = "<:to_do:751161593701728406>";
        public const string OrangeUpArrowEmoji = "<:priority_up:752663760555999392>";
        public const string BlueDownArrowEmoji = "<:priority_down:752663760556261478>";
        public const string BlueDotEmoji = "<:in_progress:752696470586654820>";
        public const string InboxEmoji = "<:claim_issue:752698049595441242>";
        public const string OutboxEmoji = "<:unclaim_issue:752698049654030437>";
        public const string GithubEmoji = "<:github:752667639968432238>";

        public static readonly Dictionary<IssueStatus, string> StatusEmojis = new Dictionary<IssueStatus, string>
        {
            [IssueStatus.ToDo] = OrangeStarEmoji,
            [IssueStatus.InProgress] = BlueDotEmoji,
            [IssueStatus.Done] = GreenCheckEmoji,
            [IssueStatus.Duplicate] = "🟫",
            [IssueStatus.Invalid] = RedXEmoji,
            [IssueStatus.WontFix] = IndeterminateEmoji
        };

        public static readonly Dictionary<IssueStatus, uint> StatusColors = new Dictionary<IssueStatus, uint>
        {
            [IssueStatus.ToDo] = 0xFF7F27,
            [IssueStatus.InProgress] = 0x3B88C3,
            [IssueStatus.Done] = 0x2CD261,
            [IssueStatus.Duplicate] = 0xC1694F,
            [IssueStatus.Invalid] = 0xEF5A73,
            [IssueStatus.WontFix] = 0xFFB144
        };

        public static readonly Dictionary<IssuePriority, string> PriorityEmojis = new Dictionary<IssuePriority, string>
        {
            [IssuePriority.Low] = "🔵",
            [IssuePriority.Medium] = "🟢",
            [IssuePriority.High] = "🟠",
            [IssuePriority.VeryHigh] = "🔴"
        };
    }
}
