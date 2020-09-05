using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class GuildOptions
    {
        public Guid Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ModeratorRoleId { get; set; }
        public ulong VoterRoleId { get; set; }
        public int MinApprovalVotes { get; set; }
        public ulong? TrackerChannelId { get; set; }
        public ulong? LoggingChannelId { get; set; }
    }
}
