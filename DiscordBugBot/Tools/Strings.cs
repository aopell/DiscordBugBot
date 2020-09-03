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

        public static readonly Dictionary<IssueStatus, string> StatusEmojis = new Dictionary<IssueStatus, string>
        {
            [IssueStatus.ToDo] = "<:issue:751161593701728406>",
            [IssueStatus.InProgress] = "⏺️",
            [IssueStatus.Done] = GreenCheckEmoji,
            [IssueStatus.Duplicate] = "🟫",
            [IssueStatus.Invalid] = RedXEmoji,
            [IssueStatus.WontFix] = IndeterminateEmoji
        };
    }
}
