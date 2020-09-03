using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class IssueCategory
    {
        public long Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public int NextNumber { get; set; }
        public string EmojiIcon { get; set; }
        public bool Archived { get; set; }
    }
}
