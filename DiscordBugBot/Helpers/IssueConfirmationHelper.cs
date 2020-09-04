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

        public static async Task HandleMessageReaction(ISocketMessageChannel channel, SocketReaction reaction, IUserMessage message)
        {
            if (!(channel is IGuildChannel gchannel)) return;
            ulong gid = gchannel.GuildId;
            ulong cid = channel.Id;
            ulong mid = message.Id;

            var options = DataStore.GetOptions(gchannel.GuildId);
            if (options is null) return;
            var user = message.Author as IGuildUser;

            if (GetModStatus(user, options, out bool mod)) return;

            IssueCategory category = GetCategory(reaction, gid);

            if (category is null) return;

            var props = DataStore.GetProposals(gid, cid, mid).ToList();

            if (props.Any(p => p.Status == ProposalStatus.Approved)) return;

            Proposal proposal = GetProposal(props, category, gid, cid, mid);

            UpdateProposals(channel, message, mod, proposal, options, props);
        }

        private static bool GetModStatus(IGuildUser user, GuildOptions options, out bool mod)
        {
            mod = false;
            if (user?.RoleIds.Contains(options.ModeratorRoleId) ?? false)
            {
                mod = true;
            }
            else if (!(user?.RoleIds.Contains(options.VoterRoleId) ?? false))
            {
                return true;
            }

            return false;
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
                _ = CreateIssue(proposal, channel, message);
            }
            else
            {
                proposal.ApprovalVotes++;
                if (proposal.ApprovalVotes >= options.MinApprovalVotes)
                {
                    _ = CreateIssue(proposal, channel, message);
                }
            }

            DataStore.UpdateProposals(props);
        }

        private static async Task CreateIssue(Proposal proposal, ISocketMessageChannel channel, IUserMessage message)
        {
            var category = DataStore.GetCategory(proposal.GuildId, proposal.Category);
            int number = category.NextNumber;
            category.NextNumber++;
            DataStore.UpdateCategory(category);
            var now = DateTimeOffset.Now;
            var issue = new Issue
            {
                GuildId = proposal.GuildId,
                ChannelId = proposal.ChannelId,
                MessageId = proposal.MessageId,
                Category = proposal.Category,
                Status = IssueStatus.ToDo,
                Title = "",
                Number = $"{category.Prefix}-{number}",
                Priority = IssuePriority.Medium,
                Assignee = null,
                Author = message.Author.Id,
                CreatedTimestamp = now,
                LastUpdatedTimestamp = now,
                Description = message.Content
            };

            DataStore.CreateIssue(issue);

            await channel.SendMessageAsync("Approved!", embed: IssueEmbedHelper.GenerateIssueEmbed(issue, category));
        }
    }
}
