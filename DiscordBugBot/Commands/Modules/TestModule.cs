using Discord.Commands;
using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands.Modules
{
    [Group("test")]
    public class TestModule : ModuleBase<BotCommandContext>
    {
        [Command("setup")]
        [Summary("Setup a server for testing")]
        public async Task Setup(string modrole, string voterrole, string logchannel, string trackerchannel)
        {
            GuildOptions options = new GuildOptions
            {
                GuildId = Context.Guild.Id,
                MinApprovalVotes = 3,
                ModeratorRoleId = Context.Message.MentionedRoles.First().Id,
                VoterRoleId = Context.Message.MentionedRoles.Skip(1).First().Id,
                LoggingChannelId = Context.Message.MentionedChannels.First().Id,
                TrackerChannelId = Context.Message.MentionedChannels.Skip(1).First().Id
            };

            Context.Bot.DataStore.CreateOptions(options);

            await Context.Channel.SendMessageAsync("Setup complete");
        }
    }
}
