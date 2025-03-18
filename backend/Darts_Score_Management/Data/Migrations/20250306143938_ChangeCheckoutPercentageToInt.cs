using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darts_Score_Management.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCheckoutPercentageToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(
            @"
            UPDATE LegStats
            SET CheckoutPercentage = CASE 
                WHEN CheckoutPercentage = '-' THEN 0
                ELSE CAST(LEFT(CheckoutPercentage, CHARINDEX('%', CheckoutPercentage) - 1) AS INT)
            END
            WHERE CheckoutPercentage IS NOT NULL;
            "
        );

            migrationBuilder.Sql(
                @"
            UPDATE SetStats
            SET CheckoutPercentage = CASE 
                WHEN CheckoutPercentage = '-' THEN 0
                ELSE CAST(LEFT(CheckoutPercentage, CHARINDEX('%', CheckoutPercentage) - 1) AS INT)
            END
            WHERE CheckoutPercentage IS NOT NULL;
            "
            );

            migrationBuilder.Sql(
                @"
            UPDATE GameStats
            SET CheckoutPercentage = CASE 
                WHEN CheckoutPercentage = '-' THEN 0
                ELSE CAST(LEFT(CheckoutPercentage, CHARINDEX('%', CheckoutPercentage) - 1) AS INT)
            END
            WHERE CheckoutPercentage IS NOT NULL;
            "
            );

            migrationBuilder.AlterColumn<int>(
                name: "CheckoutPercentage",
                table: "SetStats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CheckoutPercentage",
                table: "LegStats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CheckoutPercentage",
                table: "GameStats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @"
            UPDATE LegStats
            SET CheckoutPercentage = CAST(CheckoutPercentage AS NVARCHAR(MAX)) + '% (unknown)';
            "
        );

            migrationBuilder.Sql(
                @"
            UPDATE SetStats
            SET CheckoutPercentage = CAST(CheckoutPercentage AS NVARCHAR(MAX)) + '% (unknown)';
            "
            );

            migrationBuilder.Sql(
                @"
            UPDATE GameStats
            SET CheckoutPercentage = CAST(CheckoutPercentage AS NVARCHAR(MAX)) + '% (unknown)';
            "
            );

            migrationBuilder.AlterColumn<string>(
                name: "CheckoutPercentage",
                table: "SetStats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CheckoutPercentage",
                table: "LegStats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CheckoutPercentage",
                table: "GameStats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
