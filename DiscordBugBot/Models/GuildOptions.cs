﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class GuildOptions
    {
        // Need separate DB ID since LiteDB doesn't support ulong IDs
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong ModeratorRoleId { get; set; }
        public ulong VoterRoleId { get; set; }
        public int MinVotes { get; set; }
        public ulong? TrackerChannelId { get; set; }
        public ulong? LoggingChannelId { get; set; }
    }
}