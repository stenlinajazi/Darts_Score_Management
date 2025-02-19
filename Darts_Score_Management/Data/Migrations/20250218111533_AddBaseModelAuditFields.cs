using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darts_Score_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseModelAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Statistics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Statistics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Statistics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Statistics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Statistics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Statistics",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Statistics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Players",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImageUrl",
                table: "Players",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Players",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Players",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Players",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Games",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Games",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Games",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GamePlayers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GamePlayers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "GamePlayers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "GamePlayers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GamePlayers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "GamePlayers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "GamePlayers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "GamePlayers");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImageUrl",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
