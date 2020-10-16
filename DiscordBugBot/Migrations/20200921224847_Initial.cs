using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBugBot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildOptions",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    ModeratorRoleId = table.Column<ulong>(nullable: false),
                    VoterRoleId = table.Column<ulong>(nullable: false),
                    MinApprovalVotes = table.Column<int>(nullable: false),
                    TrackerChannelId = table.Column<ulong>(nullable: true),
                    LoggingChannelId = table.Column<ulong>(nullable: true),
                    GithubRepository = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    NextNumber = table.Column<int>(nullable: false),
                    EmojiIcon = table.Column<string>(nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_GuildOptions_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuildOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueChannels",
                columns: table => new
                {
                    ChannelId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueChannels", x => x.ChannelId);
                    table.ForeignKey(
                        name: "FK_IssueChannels_GuildOptions_GuildId",
                        column: x => x.GuildId,
                        principalTable: "GuildOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    ChannelId = table.Column<ulong>(nullable: false),
                    MessageId = table.Column<ulong>(nullable: false),
                    LogMessageId = table.Column<ulong>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Author = table.Column<ulong>(nullable: false),
                    Assignee = table.Column<ulong>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    GithubIssueNumber = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    CreatedTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    LastUpdatedTimestamp = table.Column<DateTimeOffset>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issues_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    MessageId = table.Column<ulong>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    ChannelId = table.Column<ulong>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ApprovalVotes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => new { x.MessageId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_Proposals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_GuildId_Name",
                table: "Categories",
                columns: new[] { "GuildId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueChannels_GuildId",
                table: "IssueChannels",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CategoryId",
                table: "Issues",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_GuildId_Number",
                table: "Issues",
                columns: new[] { "GuildId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_CategoryId",
                table: "Proposals",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueChannels");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "GuildOptions");
        }
    }
}
