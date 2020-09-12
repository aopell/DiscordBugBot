using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordBugBot.Models
{
    public class GuildOptions
    {
        /// <summary>
        /// Use Guild IDs for this field.
        /// </summary>
        public ulong Id { get; set; }

        public ulong ModeratorRoleId { get; set; }
        public ulong VoterRoleId { get; set; }
        public int MinApprovalVotes { get; set; }
        public ulong? TrackerChannelId { get; set; }
        public ulong? LoggingChannelId { get; set; }
        public string GithubRepository { get; set; }

        public List<GuildApprovedIssueChannel> AllowedChannels { get; set; } = new List<GuildApprovedIssueChannel>();
        public List<IssueCategory> IssueCategories { get; set; } = new List<IssueCategory>();
    }
}
