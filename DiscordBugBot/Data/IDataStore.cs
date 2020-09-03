using DiscordBugBot.Models;
using System.Collections.Generic;

namespace DiscordBugBot.Data
{
    public interface IDataStore
    {
        void SaveBug(Bug bug);
        void SaveBugCategory(BugCategory category);
        Bug GetBugById(long id);
        Bug GetBugByCategory(BugCategory category, int number);
        IEnumerable<Bug> GetBugs(ulong guild, BugCategory category = null, BugStatus? status = null, ulong? assignee = null, ulong? author = null);
        IEnumerable<Bug> GetOpenBugs(ulong guild, BugCategory category = null, ulong? assignee = null, ulong? author = null);
        BugCategory GetCategory(ulong guild, string name);
        IEnumerable<BugCategory> GetCategories(ulong guild);
        GuildOptions GetOptions(ulong guild);
        void SaveOptions(GuildOptions options);
    }
}