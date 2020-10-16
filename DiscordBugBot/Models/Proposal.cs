using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class Proposal
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ProposalStatus Status { get; set; }
        [Obsolete("Race conditions. Vote count scheme will be rennovated.")]
        public int ApprovalVotes { get; set; }

        public Guid CategoryId { get; set; }
        public IssueCategory Category { get; set; }
    }

    public enum ProposalStatus
    {
        Proposed,
        Approved
    }
}
