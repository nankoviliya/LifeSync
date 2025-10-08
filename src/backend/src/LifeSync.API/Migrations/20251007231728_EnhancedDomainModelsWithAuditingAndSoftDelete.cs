using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedDomainModelsWithAuditingAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Languages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Languages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Languages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Languages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IncomeTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "IncomeTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "IncomeTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "IncomeTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ExpenseTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExpenseTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExpenseTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ExpenseTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Currencies",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Currencies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Currencies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Currencies",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDeleted",
                table: "Languages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_IsDeleted",
                table: "IncomeTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_IsDeleted",
                table: "ExpenseTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsDeleted",
                table: "Currencies",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Languages_IsDeleted",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_IncomeTransactions_IsDeleted",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTransactions_IsDeleted",
                table: "ExpenseTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_IsDeleted",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Currencies");
        }
    }
}
