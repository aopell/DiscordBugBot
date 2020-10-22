using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBugBot.Models
{
    public class Issue
    {
        public Guid Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryIssueNumber { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ulong LogMessageId { get; set; }
        public string Number => $"{Category.Prefix}-{CategoryIssueNumber}";
        public string Title { get; set; }
        public string Description { get; set; }
        public ulong Author { get; set; }
        public ulong? Assignee { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int? GithubIssueNumber { get; set; }
        public IssueStatus Status { get; set; }
        public IssuePriority Priority { get; set; }
        public DateTimeOffset CreatedTimestamp { get; set; }
        public DateTimeOffset LastUpdatedTimestamp { get; set; }

        public Guid CategoryId { get; set; }
        public IssueCategory Category { get; set; }
    }

    public enum IssueStatus
    {
        ToDo,
        InProgress,
        Done,
        Duplicate,
        Invalid,
        WontFix
    }

    public enum IssuePriority
    {
        Low,
        Medium,
        High,
        VeryHigh
    }
}
