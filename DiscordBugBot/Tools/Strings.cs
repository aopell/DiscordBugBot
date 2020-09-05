using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Tools
{
    public static class Strings
    {
        public const string QuestionEmoji = "<:unknown:706271972983701524>";
        public const string BlueCheckEmoji = "<:ready:706270984973451295>";
        public const string GreenCheckEmoji = "<:available:706270615312662568>";
        public const string IndeterminateEmoji = "<:maybe:706702223446376517>";
        public const string RedXEmoji = "<:unavailable:706702240467124345>";
        public const string OrangeStarEmoji = "<:issue:751161593701728406>";

        public const string DenyReaction = "❌";

        public static readonly Dictionary<IssueStatus, string> StatusEmojis = new Dictionary<IssueStatus, string>
        {
            [IssueStatus.ToDo] = OrangeStarEmoji,
            [IssueStatus.InProgress] = "⏺️",
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
