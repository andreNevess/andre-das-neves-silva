using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequestFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Requester = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequests", x => x.Id);
                    table.CheckConstraint("CK_SupportRequests_CompletedAtUtc", "([Status] = 'Completed' AND [CompletedAtUtc] IS NOT NULL) OR ([Status] <> 'Completed' AND [CompletedAtUtc] IS NULL)");
                    table.CheckConstraint("CK_SupportRequests_Priority", "[Priority] IN ('Low', 'Medium', 'High')");
                    table.CheckConstraint("CK_SupportRequests_Status", "[Status] IN ('Open', 'InProgress', 'Completed')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_Requester",
                table: "SupportRequests",
                column: "Requester");

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_Status_Priority_CreatedAtUtc",
                table: "SupportRequests",
                columns: new[] { "Status", "Priority", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_Title",
                table: "SupportRequests",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportRequests");
        }
    }
}
