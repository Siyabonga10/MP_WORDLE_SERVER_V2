using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MP_WORDLE_SERVER_V2.Migrations
{
    /// <inheritdoc />
    public partial class HostAndWinnerID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WinnerID",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "WinnerID",
                table: "Games");
        }
    }
}
