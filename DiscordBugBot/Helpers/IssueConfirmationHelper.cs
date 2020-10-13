using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBugBot.Data;
using DiscordBugBot.Models;
using DiscordBugBot.Tools;
using Microsoft.EntityFrameworkCore;

namespace DiscordBugBot.Helpers
{
    public class IssueConfirmationHelper
    {
        private IssueHelper helper;
        private BugBotDataContext dataStore;

        public IssueConfirmationHelper(BugBotDataContext dataStore, IssueHelper helper)
        {
            this.dataStore = dataStore;
            this.helper = helper;
        }

        public async Task HandleMessageReaction(ISocketMessageChannel channel, SocketReaction reaction, IUserMessage message)
        {
            if ((reaction.User.GetValueOrDefault()?.IsBot ?? false) || !(channel is IGuildChannel gchannel)) return;
            ulong gid = gchannel.GuildId;
            ulong cid = channel.Id;
            ulong mid = message.Id;

            var options = await dataStore.GuildOptions.Include(g => g.AllowedChannels).SingleOrDefaultAsync(x => x.Id == gchannel.GuildId);
            if (options?.AllowedChannels is null || !options.AllowedChannels.Any(x => x.ChannelId == channel.Id)) return;
            var user = reaction.User.Value as IGuildUser;

            (bool voter, bool mod) = GetVoterStatus(user, options);
            if (!voter) return;

            IssueCategory category = GetCategory(reaction, gid);

            if (category is null) return;

            if (await ((IQueryable<Proposal>)dataStore.Proposals).AnyAsync(p => p.GuildId == gid && p.ChannelId == cid && p.MessageId == mid && p.Status == ProposalStatus.Approved)) return;

            var proposal = dataStore.Proposals.FirstOrDefault(p => p.GuildId == gid && p.ChannelId == cid && p.MessageId == mid && p.CategoryId == category.Id);
            if (proposal is null)
            {
                proposal = new Proposal
                {
                    GuildId = gid,
                    ChannelId = cid,
                    MessageId = mid,
                    CategoryId = category.Id,
                    Status = ProposalStatus.Proposed
                };
                dataStore.Add(proposal);
            }

            await UpdateProposals(channel, message, mod, proposal, options);

            await dataStore.SaveChangesAsync();
        }

        public static (bool voter, bool mod) GetVoterStatus(IGuildUser user, GuildOptions options)
        {
            bool mod = false;
            bool voter = false;
            if (user?.RoleIds.Contains(options.ModeratorRoleId) ?? false)
            {
                mod = true;
                voter = true;
            }
            if (user?.RoleIds.Contains(options.VoterRoleId) ?? false)
            {
                voter = true;
            }

            return (voter, mod);
        }

        private IssueCategory GetCategory(SocketReaction reaction, ulong gid)
        {
            string emoteStr = reaction.Emote.ToString();
            return dataStore.Categories.SingleOrDefault(c => c.GuildId == gid && c.EmojiIcon == emoteStr);
        }


        private async Task UpdateProposals(
            ISocketMessageChannel channel,
            IUserMessage message,
            bool mod,
            Proposal proposal,
            GuildOptions options
        )
        {
            if (mod)
            {
                proposal.Status = ProposalStatus.Approved;
                await helper.CreateIssue(proposal, channel, message, options);
            }
            else
            {
                proposal.ApprovalVotes++;
                if (proposal.ApprovalVotes >= options.MinApprovalVotes)
                {
                    await helper.CreateIssue(proposal, channel, message, options);
                }
            }

            dataStore.SaveChanges();
        }
    }
}
