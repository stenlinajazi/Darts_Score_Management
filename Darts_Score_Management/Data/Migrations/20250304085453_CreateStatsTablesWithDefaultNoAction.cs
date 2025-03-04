using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darts_Score_Management.Migrations
{
    /// <inheritdoc />
    public partial class CreateStatsTablesWithDefaultNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayers_Players_PlayerId",
                table: "GamePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Legs_Players_WinnerPlayerId",
                table: "Legs");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_Players_WinnerPlayerId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Turns_Players_PlayerId",
                table: "Turns");

            migrationBuilder.CreateTable(
                name: "GameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GamePlayerId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    SetsWin = table.Column<int>(type: "int", nullable: false),
                    LegsWin = table.Column<int>(type: "int", nullable: false),
                    PPD = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    First9PPD = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    CheckoutPercentage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count60Plus = table.Column<int>(type: "int", nullable: false),
                    Count100Plus = table.Column<int>(type: "int", nullable: false),
                    Count140Plus = table.Column<int>(type: "int", nullable: false),
                    Count180s = table.Column<int>(type: "int", nullable: false),
                    HighestCheckout = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameStats_GamePlayers_GamePlayerId",
                        column: x => x.GamePlayerId,
                        principalTable: "GamePlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameStats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LegStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GamePlayerId = table.Column<int>(type: "int", nullable: false),
                    LegId = table.Column<int>(type: "int", nullable: false),
                    PPD = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    First9PPD = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    TotalThrows = table.Column<int>(type: "int", nullable: false),
                    CheckoutPercentage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count60Plus = table.Column<int>(type: "int", nullable: false),
                    Count100Plus = table.Column<int>(type: "int", nullable: false),
                    Count140Plus = table.Column<int>(type: "int", nullable: false),
                    Count180s = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegStats_GamePlayers_GamePlayerId",
                        column: x => x.GamePlayerId,
                        principalTable: "GamePlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegStats_Legs_LegId",
                        column: x => x.LegId,
                        principalTable: "Legs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SetStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GamePlayerId = table.Column<int>(type: "int", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false),
                    LegsWin = table.Column<int>(type: "int", nullable: false),
                    PPD = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    First9PPD = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    CheckoutPercentage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count60Plus = table.Column<int>(type: "int", nullable: false),
                    Count100Plus = table.Column<int>(type: "int", nullable: false),
                    Count140Plus = table.Column<int>(type: "int", nullable: false),
                    Count180s = table.Column<int>(type: "int", nullable: false),
                    HighestCheckout = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetStats_GamePlayers_GamePlayerId",
                        column: x => x.GamePlayerId,
                        principalTable: "GamePlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetStats_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GameId",
                table: "GameStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GamePlayerId",
                table: "GameStats",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LegStats_GamePlayerId",
                table: "LegStats",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LegStats_LegId",
                table: "LegStats",
                column: "LegId");

            migrationBuilder.CreateIndex(
                name: "IX_SetStats_GamePlayerId",
                table: "SetStats",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_SetStats_SetId",
                table: "SetStats",
                column: "SetId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayers_Players_PlayerId",
                table: "GamePlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Legs_Players_WinnerPlayerId",
                table: "Legs",
                column: "WinnerPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_Players_WinnerPlayerId",
                table: "Sets",
                column: "WinnerPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Turns_Players_PlayerId",
                table: "Turns",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayers_Players_PlayerId",
                table: "GamePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Legs_Players_WinnerPlayerId",
                table: "Legs");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_Players_WinnerPlayerId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Turns_Players_PlayerId",
                table: "Turns");

            migrationBuilder.DropTable(
                name: "GameStats");

            migrationBuilder.DropTable(
                name: "LegStats");

            migrationBuilder.DropTable(
                name: "SetStats");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayers_Players_PlayerId",
                table: "GamePlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Legs_Players_WinnerPlayerId",
                table: "Legs",
                column: "WinnerPlayerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_Players_WinnerPlayerId",
                table: "Sets",
                column: "WinnerPlayerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Turns_Players_PlayerId",
                table: "Turns",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
