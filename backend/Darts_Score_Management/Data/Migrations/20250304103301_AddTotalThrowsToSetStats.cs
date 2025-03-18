using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darts_Score_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalThrowsToSetStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalThrows",
                table: "SetStats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalThrows",
                table: "SetStats");
        }
    }
}
