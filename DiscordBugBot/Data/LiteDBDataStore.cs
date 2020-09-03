using System;
using System.Collections.Generic;
using System.Text;
using DiscordBugBot.Models;

namespace DiscordBugBot.Data
{
    public class LiteDbDataStore : IDataStore
    {
        private string FilePath { get; }
        private const string BugCollectionName = "bugs";
        private const string CategoryCollectionName = "categories";
        private const string OptionsCollectionName = "options";

        public LiteDbDataStore(string filePath)
        {
            FilePath = filePath;
        }

        public void SaveBug(Bug bug)
        {
            throw new NotImplementedException();
        }

        public void SaveBugCategory(BugCategory category)
        {
            throw new NotImplementedException();
        }

        public Bug GetBugById(long id) => throw new NotImplementedException();

        public Bug GetBugByCategory(BugCategory category, int number) => throw new NotImplementedException();

        public IEnumerable<Bug> GetBugs(ulong guild, BugCategory category = null, BugStatus? status = null, ulong? assignee = null, ulong? author = null) => throw new NotImplementedException();

        public IEnumerable<Bug> GetOpenBugs(ulong guild, BugCategory category = null, ulong? assignee = null, ulong? author = null) => throw new NotImplementedException();

        public BugCategory GetCategory(ulong guild, string name) => throw new NotImplementedException();

        public IEnumerable<BugCategory> GetCategories(ulong guild) => throw new NotImplementedException();
        public GuildOptions GetOptions(ulong guild) => throw new NotImplementedException();

        public void SaveOptions(GuildOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
