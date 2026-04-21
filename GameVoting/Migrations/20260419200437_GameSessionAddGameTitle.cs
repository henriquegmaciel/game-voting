using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameVoting.Migrations
{
    /// <inheritdoc />
    public partial class GameSessionAddGameTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameTitle",
                table: "GameSessions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameTitle",
                table: "GameSessions");
        }
    }
}
