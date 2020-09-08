using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;

namespace DiscordBugBot.Models
{
    [NamedArgumentType]
    public class IssueUpdateArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IUser Assignee { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }

        public static ISelfUser NoUser => DiscordBot.MainInstance.Client.CurrentUser;
    }
}
