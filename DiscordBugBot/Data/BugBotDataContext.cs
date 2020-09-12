using DiscordBugBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBugBot.Data
{
    public class BugBotDataContext : DbContext
    {
        private static string SqliteConnectionString = "Data Source=issues_sqlite.db";

        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueCategory> Categories { get; set; }
        public DbSet<GuildOptions> GuildOptions { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<GuildApprovedIssueChannel> IssueChannels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(SqliteConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Issue>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Issues);

            modelBuilder.Entity<Issue>()
                .HasIndex(i => new { i.GuildId, i.Number })
                .IsUnique();

            modelBuilder.Entity<Proposal>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Proposals);

            modelBuilder.Entity<Proposal>().HasKey(p => new { p.MessageId, p.CategoryId });

            modelBuilder.Entity<GuildApprovedIssueChannel>()
                .HasOne(aic => aic.Guild)
                .WithMany(g => g.AllowedChannels)
                .HasForeignKey(aic => aic.GuildId)
                .HasPrincipalKey(g => g.Id);

            modelBuilder.Entity<IssueCategory>()
                .HasIndex(c => new { c.GuildId, c.Name })
                .IsUnique();

            modelBuilder.Entity<IssueCategory>()
                .HasOne(c => c.Guild)
                .WithMany(g => g.IssueCategories)
                .HasForeignKey(c => c.GuildId)
                .HasPrincipalKey(g => g.Id);
        }
    }
}