using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darts_Score_Management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatisticsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameStats_Games_GameId",
                table: "GameStats");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropIndex(
                name: "IX_SetStats_GamePlayerId",
                table: "SetStats");

            migrationBuilder.DropIndex(
                name: "IX_LegStats_GamePlayerId",
                table: "LegStats");

            migrationBuilder.DropIndex(
                name: "IX_GameStats_GameId",
                table: "GameStats");

            migrationBuilder.DropIndex(
                name: "IX_GameStats_GamePlayerId",
                table: "GameStats");

            migrationBuilder.CreateIndex(
                name: "IX_Turns_LegId",
                table: "Turns",
                column: "LegId");

            migrationBuilder.CreateIndex(
                name: "IX_SetStats_GamePlayerId_SetId",
                table: "SetStats",
                columns: new[] { "GamePlayerId", "SetId" });

            migrationBuilder.CreateIndex(
                name: "IX_Sets_GameId",
                table: "Sets",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LegStats_GamePlayerId_LegId",
                table: "LegStats",
                columns: new[] { "GamePlayerId", "LegId" });

            migrationBuilder.CreateIndex(
                name: "IX_Legs_SetId",
                table: "Legs",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GamePlayerId_GameId",
                table: "GameStats",
                columns: new[] { "GamePlayerId", "GameId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turns_LegId",
                table: "Turns");

            migrationBuilder.DropIndex(
                name: "IX_SetStats_GamePlayerId_SetId",
                table: "SetStats");

            migrationBuilder.DropIndex(
                name: "IX_Sets_GameId",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_LegStats_GamePlayerId_LegId",
                table: "LegStats");

            migrationBuilder.DropIndex(
                name: "IX_Legs_SetId",
                table: "Legs");

            migrationBuilder.DropIndex(
                name: "IX_GameStats_GamePlayerId_GameId",
                table: "GameStats");

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GamePlayerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistics_GamePlayers_GamePlayerId",
                        column: x => x.GamePlayerId,
                        principalTable: "GamePlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetStats_GamePlayerId",
                table: "SetStats",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LegStats_GamePlayerId",
                table: "LegStats",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GameId",
                table: "GameStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GamePlayerId",
                table: "GameStats",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_GamePlayerId",
                table: "Statistics",
                column: "GamePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameStats_Games_GameId",
                table: "GameStats",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
