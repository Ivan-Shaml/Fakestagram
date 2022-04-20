using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fakestagram.Migrations
{
    public partial class RefreshTokensJwtId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JwtId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JwtId",
                table: "RefreshTokens");
        }
    }
}
