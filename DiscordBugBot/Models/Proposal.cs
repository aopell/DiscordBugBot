using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class Proposal
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string Category { get; set; }
        public ProposalStatus Status { get; set; }
        public int ApprovalVotes { get; set; }
    }

    public enum ProposalStatus
    {
        Proposed,
        Approved
    }
}
