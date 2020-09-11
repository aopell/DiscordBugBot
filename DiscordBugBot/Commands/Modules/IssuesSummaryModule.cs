using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands.Modules
{
    [Group("issues")]
    public class IssuesSummaryModule
    {
        [ModeratorRequired]
        [Command("by")]
        [Summary("Lists issues authored by the specified user")]
        public async Task AuthorIssues(IUser user, bool openOnly = true)
        {

        }

        [ModeratorRequired]
        [Command("for")]
        [Summary("Lists issues assigned to the specified user")]
        public async Task UserIssues(IUser user, bool openOnly = true)
        {

        }

        [ModeratorRequired]
        [Command("mine")]
        [Alias("me")]
        [Summary("Lists issues assigned to you")]
        public async Task Mine(bool openOnly = true)
        {

        }

        [VoterRequired]
        [Command]
        [Summary("Lists all issues")]
        public async Task Open(string category = null, bool openOnly = false)
        {

        }
    }
}
