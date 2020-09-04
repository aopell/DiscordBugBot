using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DiscordBugBot.Models;
using LiteDB;

namespace DiscordBugBot.Data
{
    public class LiteDbDataStore : IDataStore
    {
        private string FilePath { get; }
        private const string IssueCollectionName = "issues";
        private const string CategoryCollectionName = "categories";
        private const string OptionsCollectionName = "options";
        private const string ProposalsCollectionName = "proposals";

        public LiteDbDataStore(string filePath)
        {
            FilePath = filePath;
        }

        public void CreateIssue(Issue issue)
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            issues.Insert(issue);
        }

        public void UpdateIssue(Issue issue)
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            issues.Update(issue);
        }

        public void CreateCategory(IssueCategory category)
        {
            using var db = new LiteDatabase(FilePath);
            var categories = db.GetCollection<IssueCategory>(CategoryCollectionName);
            categories.Insert(category);
        }

        public void UpdateCategory(IssueCategory category)
        {
            using var db = new LiteDatabase(FilePath);
            var categories = db.GetCollection<IssueCategory>(CategoryCollectionName);
            categories.Update(category);
        }

        public Issue GetIssueById(long id)
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            return issues.FindOne(x => x.Id == id);
        }

        public Issue GetIssueByMessage(ulong guild, ulong channel, ulong message)
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            return issues.FindOne(x => x.GuildId == guild && x.ChannelId == channel && x.MessageId == message);
        }

        public Issue GetIssueByCategory(IssueCategory category, string number)
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            return issues.FindOne(x => x.Category == category.Name && x.Number == number);
        }

        public IEnumerable<Issue> GetIssues(
            ulong guild,
            IssueCategory category = null,
            IssueStatus? status = null,
            ulong? assignee = null,
            ulong? author = null
        )
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            return issues.Find(x =>
               x.GuildId == guild
               && (category == null || x.Category == category.Name)
               && (status == null || x.Status == status)
               && (assignee == null || x.Assignee == assignee)
               && (author == null || x.Author == author)
            ).ToList();
        }

        public IEnumerable<Issue> GetOpenIssues(
            ulong guild,
            IssueCategory category = null,
            ulong? assignee = null,
            ulong? author = null
        )
        {
            using var db = new LiteDatabase(FilePath);
            var issues = db.GetCollection<Issue>(IssueCollectionName);
            return issues.Find(x =>
                 x.GuildId == guild
                 && (x.Status == IssueStatus.ToDo || x.Status == IssueStatus.InProgress)
                 && (category == null || x.Category == category.Name)
                 && (assignee == null || x.Assignee == assignee)
                 && (author == null || x.Author == author)
            ).ToList();
        }

        public IssueCategory GetCategory(ulong guild, string name)
        {
            using var db = new LiteDatabase(FilePath);
            var categories = db.GetCollection<IssueCategory>(CategoryCollectionName);
            return categories.FindOne(x => x.GuildId == guild && x.Name == name);
        }

        public IEnumerable<IssueCategory> GetCategories(ulong guild)
        {
            using var db = new LiteDatabase(FilePath);
            var categories = db.GetCollection<IssueCategory>(CategoryCollectionName);
            return categories.Find(x => x.GuildId == guild).ToList();
        }

        public GuildOptions GetOptions(ulong guild)
        {
            using var db = new LiteDatabase(FilePath);
            var options = db.GetCollection<GuildOptions>(OptionsCollectionName);
            return options.FindOne(x => x.GuildId == guild);
        }

        public void CreateOptions(GuildOptions guildOptions)
        {
            using var db = new LiteDatabase(FilePath);
            var options = db.GetCollection<GuildOptions>(OptionsCollectionName);
            options.Insert(guildOptions);
        }

        public void UpdateOptions(GuildOptions guildOptions)
        {
            using var db = new LiteDatabase(FilePath);
            var options = db.GetCollection<GuildOptions>(OptionsCollectionName);
            options.Update(guildOptions);
        }

        public IEnumerable<Proposal> GetProposals(ulong guild, ulong channel, ulong message)
        {
            using var db = new LiteDatabase(FilePath);
            var proposals = db.GetCollection<Proposal>(ProposalsCollectionName);
            return proposals.Find(x => x.GuildId == guild && x.ChannelId == channel && x.MessageId == message).ToList();
        }

        public void CreateProposal(Proposal proposal)
        {
            using var db = new LiteDatabase(FilePath);
            var proposals = db.GetCollection<Proposal>(ProposalsCollectionName);
            proposals.Insert(proposal);
        }

        public void UpdateProposals(IEnumerable<Proposal> proposals)
        {
            using var db = new LiteDatabase(FilePath);
            var proposalsCollection = db.GetCollection<Proposal>(ProposalsCollectionName);
            proposalsCollection.Update(proposals);
        }
    }
}
