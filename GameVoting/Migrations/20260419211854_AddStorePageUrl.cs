using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameVoting.Migrations
{
    /// <inheritdoc />
    public partial class AddStorePageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorePageUrl",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorePageUrl",
                table: "Games");
        }
    }
}
