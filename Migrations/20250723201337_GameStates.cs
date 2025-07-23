using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MP_WORDLE_SERVER_V2.Migrations
{
    /// <inheritdoc />
    public partial class GameStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Games");
        }
    }
}
