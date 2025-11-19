using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Den.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Period = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Provider = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetBudgetSource",
                columns: table => new
                {
                    BudgetSourcesId = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetBudgetSource", x => new { x.BudgetSourcesId, x.BudgetsId });
                    table.ForeignKey(
                        name: "FK_BudgetBudgetSource_BudgetSources_BudgetSourcesId",
                        column: x => x.BudgetSourcesId,
                        principalTable: "BudgetSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetBudgetSource_Budgets_BudgetsId",
                        column: x => x.BudgetsId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetPoints_BudgetSources_BudgetSourceId",
                        column: x => x.BudgetSourceId,
                        principalTable: "BudgetSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetPoints_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetBudgetSource_BudgetsId",
                table: "BudgetBudgetSource",
                column: "BudgetsId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPoints_BudgetId",
                table: "BudgetPoints",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPoints_BudgetSourceId",
                table: "BudgetPoints",
                column: "BudgetSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_OwnerId",
                table: "Budgets",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetBudgetSource");

            migrationBuilder.DropTable(
                name: "BudgetPoints");

            migrationBuilder.DropTable(
                name: "BudgetSources");

            migrationBuilder.DropTable(
                name: "Budgets");
        }
    }
}
