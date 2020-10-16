using Discord;
using Discord.Commands;
using DiscordBugBot.Data;
using DiscordBugBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands.Modules
{
    [Group("setup")]
    public class SetupModule : ModuleBase<SocketCommandContext>
    {
        public BugBotDataContext DataStore { get; set; }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Command]
        [Summary("Provides information on how to set up the server")]
        public async Task Setup()
        {
            await Context.Channel.SendMessageAsync(
                "**__Welcome to BugBot!__**\nCreate a channel to log issue tickets as they're created, " +
                "and decide on which role to treat as issue moderators (they have the authority to create and" +
                " modify tickets at will) and another role that has permission to vote on tickets. " +
                "You can also define the number of votes necessary to create an issue.\n\n" +
                "**Once you're ready, run the command:**\n`setup <#issue-log-channel> <#dashboard-channel> <@mod-role> <@voter-role> [min-votes = 3] [github-repository = null]`"
            );
        }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Command]
        [Summary("Sets up the server with the given options")]
        public async Task Setup(IMessageChannel issueLogChannel, IMessageChannel dashboardChannel, IRole modRole, IRole voterRole, int minVotes = 3, string githubRepository = null)
        {
            using (var tx = await DataStore.Database.BeginTransactionAsync())
            {
                GuildOptions existingOpts = DataStore.GuildOptions.Find(Context.Guild.Id);
                var options = existingOpts ?? new GuildOptions();

                options.Id = Context.Guild.Id;
                options.LoggingChannelId = issueLogChannel.Id;
                options.TrackerChannelId = dashboardChannel.Id;
                options.ModeratorRoleId = modRole.Id;
                options.VoterRoleId = voterRole.Id;
                options.MinApprovalVotes = minVotes;
                options.GithubRepository = githubRepository;

                if (existingOpts != null)
                {
                    DataStore.GuildOptions.Update(options);
                }
                else
                {
                    DataStore.GuildOptions.Add(options);
                }
                await DataStore.SaveChangesAsync();

                await tx.CommitAsync();
            }
            await Context.Channel.SendMessageAsync("Settings created or updated successfully");
        }

        [Group("channel")]
        public class ChannelSetupModule : ModuleBase<SocketCommandContext>
        {
            public BugBotDataContext DataStore { get; set; }

            [ModeratorRequired]
            [Command]
            [Summary("Sets up the current channel as a valid proposal channel")]
            public async Task Channel(IMessageChannel channel = null)
            {
                channel ??= Context.Channel;
                DataStore.IssueChannels.Add(new GuildApprovedIssueChannel() { GuildId = Context.Guild.Id, ChannelId = channel.Id });
                await DataStore.SaveChangesAsync();
                await Context.Channel.SendMessageAsync("Channel set up successfully");
            }

            [ModeratorRequired]
            [Command("remove")]
            [Summary("Removes the current channel from the valid proposal channels")]
            public async Task Remove(IMessageChannel channel = null)
            {
                channel ??= Context.Channel;
                // https://entityframeworkcore.com/knowledge-base/47813464/delete-loaded-and-unloaded-objects-by-id-in-entityframeworkcore
                var instance = await DataStore.IssueChannels.SingleAsync(x => x.GuildId == Context.Guild.Id && x.ChannelId == channel.Id);
                DataStore.Remove<GuildApprovedIssueChannel>(instance);
                await DataStore.SaveChangesAsync();
                //if (!options.AllowedChannels.Remove(channel.Id)) throw new CommandExecutionException("That channel is not a proposal channel");
                await Context.Channel.SendMessageAsync("Channel removed successfully");
            }
        }
    }
}
