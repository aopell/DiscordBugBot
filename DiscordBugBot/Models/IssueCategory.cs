using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Models
{
    public class IssueCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string EmojiIcon { get; set; }
        public bool Archived { get; set; }

        public ulong GuildId { get; set; }
        public GuildOptions Guild { get; set; }

        public List<Issue> Issues { get; set; } = new List<Issue>();
        public List<Proposal> Proposals { get; set; } = new List<Proposal>();
    }
}
