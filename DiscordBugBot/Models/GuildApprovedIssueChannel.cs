using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBugBot.Models
{
    public class GuildApprovedIssueChannel
    {
        // I believe ChannelIDs are snowflakes, so should be globally unique
        // They may however collide with other types of keys
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        public GuildOptions Guild { get; set; }
    }
}
