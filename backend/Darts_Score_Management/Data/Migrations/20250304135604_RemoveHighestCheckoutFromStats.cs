using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darts_Score_Management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHighestCheckoutFromStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighestCheckout",
                table: "SetStats");

            migrationBuilder.DropColumn(
                name: "HighestCheckout",
                table: "GameStats");

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckoutAttempt",
                table: "Turns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheckoutAttempt",
                table: "Turns");

            migrationBuilder.AddColumn<int>(
                name: "HighestCheckout",
                table: "SetStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HighestCheckout",
                table: "GameStats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
