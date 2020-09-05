using Discord.Commands;
using Discord.WebSocket;
using DiscordBugBot.Helpers;
using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands.Modules
{
    [Group("issue")]
    public class IssueModule : ModuleBase<BotCommandContext>
    {
        [Command("create")]
        [Summary("Create an issue")]
        public async Task Create(string category, string title, [Remainder] string description)
        {
            Proposal proposal = new Proposal
            {
                Category = category,
                ChannelId = Context.Channel.Id,
                GuildId = Context.Guild.Id,
                MessageId = Context.Message.Id,
                Status = ProposalStatus.Approved
            };

            GuildOptions options = Context.Bot.DataStore.GetOptions(Context.Guild.Id);

            IssueConfirmationHelper.CreateIssue(proposal, Context.Channel as ISocketMessageChannel, Context.Message, options, title: title, description: description);
        }
    }
}
