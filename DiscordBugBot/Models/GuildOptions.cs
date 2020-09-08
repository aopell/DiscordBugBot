using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        public string GithubRepository { get; set; }
        public List<ulong> AllowedChannels { get; set; } = new List<ulong>();
    }
}
