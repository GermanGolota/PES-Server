using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class ChatRolesRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminToChat");

            migrationBuilder.DropColumn(
                name: "RefreshTokenId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "UserToChat",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Users_UserId",
                table: "Tokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Users_UserId",
                table: "Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_UserId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserToChat");

            migrationBuilder.AddColumn<Guid>(
                name: "RefreshTokenId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AdminToChat",
                columns: table => new
                {
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminToChat", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AdminToChat_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
