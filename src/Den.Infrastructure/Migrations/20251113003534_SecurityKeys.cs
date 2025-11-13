using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Den.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecurityKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EncryptedPrivateKey = table.Column<string>(type: "text", nullable: false),
                    KeyType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    HashAlgorithm = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PublicJwkJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityKeys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityKeys_IsActive_IsRevoked_ExpiresAt",
                table: "SecurityKeys",
                columns: new[] { "IsActive", "IsRevoked", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityKeys_KeyId",
                table: "SecurityKeys",
                column: "KeyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityKeys");
        }
    }
}
