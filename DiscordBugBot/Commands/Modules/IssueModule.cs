﻿using Discord.Commands;
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

            if (options == null)
            {
                throw new CommandExecutionException("Please set up the server before trying to create issues. Run the `setup` command to get started.");
            }

            Context.Bot.DataStore.CreateProposal(proposal);
            await IssueConfirmationHelper.CreateIssue(proposal, Context.Channel, Context.Message, options, title: title, description: description);
        }

        [Command]
        [Summary("Gets an issue")]
        public async Task Get(string number)
        {
            Issue issue = Context.Bot.DataStore.GetIssueByNumber(number);
            if (issue == null)
            {
                throw new CommandExecutionException("That issue does not exist");
            }
            await Context.Channel.SendMessageAsync(embed: IssueEmbedHelper.GenerateInlineIssueEmbed(issue));
        }
    }
}