using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class Bug
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string Category { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ulong Author { get; set; }
        public ulong Assignee { get; set; }
        public BugStatus Status { get; set; }
        public BugPriority Priority { get; set; }
        public DateTimeOffset CreatedTimestamp { get; set; }
        public DateTimeOffset LastUpdatedTimestamp { get; set; }
    }

    public enum BugStatus
    {
        Proposed,
        ToDo,
        InProgress,
        Done,
        Duplicate,
        Invalid,
        Declined
    }

    public enum BugPriority
    {
        Low,
        Medium,
        High,
        VeryHigh
    }
}
