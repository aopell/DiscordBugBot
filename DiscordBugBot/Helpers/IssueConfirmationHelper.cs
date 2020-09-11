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

namespace DiscordBugBot.Helpers
{
    public static class IssueConfirmationHelper
    {
        private static IDataStore DataStore => DiscordBot.MainInstance.DataStore;
        private static DiscordSocketClient Client => DiscordBot.MainInstance.Client;

        public static async Task HandleMessageReaction(ISocketMessageChannel channel, SocketReaction reaction, IUserMessage message)
        {
            if ((reaction.User.GetValueOrDefault()?.IsBot ?? false) || !(channel is IGuildChannel gchannel)) return;
            ulong gid = gchannel.GuildId;
            ulong cid = channel.Id;
            ulong mid = message.Id;

            var options = DataStore.GetOptions(gchannel.GuildId);
            if (options?.AllowedChannels is null || !options.AllowedChannels.Contains(channel.Id)) return;
            var user = reaction.User.Value as IGuildUser;

            (bool voter, bool mod) = GetVoterStatus(user, options);
            if (!voter) return;

            IssueCategory category = GetCategory(reaction, gid);

            if (category is null) return;

            var props = DataStore.GetProposals(gid, cid, mid).ToList();

            if (props.Any(p => p.Status == ProposalStatus.Approved)) return;

            Proposal proposal = GetProposal(props, category, gid, cid, mid);

            UpdateProposals(channel, message, mod, proposal, options, props);
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

        private static IssueCategory GetCategory(SocketReaction reaction, ulong gid)
        {
            var categories = DataStore.GetCategories(gid);
            var category = categories.FirstOrDefault(c => c.EmojiIcon == reaction.Emote.ToString());
            return category;
        }

        private static Proposal GetProposal(List<Proposal> props, IssueCategory category, ulong gid, ulong cid, ulong mid)
        {
            var proposal = props.FirstOrDefault(p => p.Category == category.Name);
            if (proposal is null)
            {
                proposal = new Proposal
                {
                    GuildId = gid,
                    ChannelId = cid,
                    MessageId = mid,
                    Category = category.Name,
                    Status = ProposalStatus.Proposed
                };
                DataStore.CreateProposal(proposal);
            }

            return proposal;
        }

        private static void UpdateProposals(
            ISocketMessageChannel channel,
            IUserMessage message,
            bool mod,
            Proposal proposal,
            GuildOptions options,
            List<Proposal> props
        )
        {
            if (mod)
            {
                proposal.Status = ProposalStatus.Approved;
                _ = IssueModificationHelper.CreateIssue(proposal, channel, message, options);
            }
            else
            {
                proposal.ApprovalVotes++;
                if (proposal.ApprovalVotes >= options.MinApprovalVotes)
                {
                    _ = IssueModificationHelper.CreateIssue(proposal, channel, message, options);
                }
            }

            DataStore.UpdateProposals(props);
        }
    }
}
