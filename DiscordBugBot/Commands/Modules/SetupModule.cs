using Discord;
using Discord.Commands;
using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands.Modules
{
    [Group("setup")]
    public class SetupModule : ModuleBase<BotCommandContext>
    {
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
            bool exists = true;
            var options = Context.Bot.DataStore.GetOptions(Context.Guild.Id);
            if (options is null)
            {
                exists = false;
                options = new GuildOptions();
            }

            options.GuildId = Context.Guild.Id;
            options.LoggingChannelId = issueLogChannel.Id;
            options.TrackerChannelId = dashboardChannel.Id;
            options.ModeratorRoleId = modRole.Id;
            options.VoterRoleId = voterRole.Id;
            options.MinApprovalVotes = minVotes;
            options.GithubRepository = githubRepository;

            if (exists)
            {
                Context.Bot.DataStore.UpdateOptions(options);
            }
            else
            {
                Context.Bot.DataStore.CreateOptions(options);
            }

            await Context.Channel.SendMessageAsync("Settings created or updated successfully");
        }

        [Group("channel")]
        public class ChannelSetupModule : ModuleBase<BotCommandContext>
        {
            [ModeratorRequired]
            [Command]
            [Summary("Sets up the current channel as a valid proposal channel")]
            public async Task Channel(IMessageChannel channel = null)
            {
                channel ??= Context.Channel;
                var options = Context.Bot.DataStore.GetOptions(Context.Guild.Id);
                if (options is null) throw new CommandExecutionException("Please run `setup` before running this command");
                options.AllowedChannels ??= new List<ulong>();
                if (options.AllowedChannels.Contains(channel.Id)) throw new CommandExecutionException("That channel is already a proposal channel");
                options.AllowedChannels.Add(channel.Id);
                Context.Bot.DataStore.UpdateOptions(options);
                await Context.Channel.SendMessageAsync("Channel set up successfully");
            }

            [ModeratorRequired]
            [Command("remove")]
            [Summary("Removes the current channel from the valid proposal channels")]
            public async Task Remove(IMessageChannel channel = null)
            {
                channel ??= Context.Channel;
                var options = Context.Bot.DataStore.GetOptions(Context.Guild.Id);
                if (options is null) throw new CommandExecutionException("Please run `setup` before running this command");
                options.AllowedChannels ??= new List<ulong>();
                if (!options.AllowedChannels.Remove(channel.Id)) throw new CommandExecutionException("That channel is not a proposal channel");
                Context.Bot.DataStore.UpdateOptions(options);
                await Context.Channel.SendMessageAsync("Channel removed successfully");
            }
        }
    }
}
