using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Den.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecurityKeysUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SecurityKeys_IsActive_IsRevoked_ExpiresAt",
                table: "SecurityKeys");

            migrationBuilder.AddColumn<string>(
                name: "Usage",
                table: "SecurityKeys",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityKeys_IsActive_IsRevoked_ExpiresAt_Usage",
                table: "SecurityKeys",
                columns: new[] { "IsActive", "IsRevoked", "ExpiresAt", "Usage" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SecurityKeys_IsActive_IsRevoked_ExpiresAt_Usage",
                table: "SecurityKeys");

            migrationBuilder.DropColumn(
                name: "Usage",
                table: "SecurityKeys");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityKeys_IsActive_IsRevoked_ExpiresAt",
                table: "SecurityKeys",
                columns: new[] { "IsActive", "IsRevoked", "ExpiresAt" });
        }
    }
}
