using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCurrenciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add new columns with temporary nullable constraint to allow data migration
            migrationBuilder.AddColumn<string>(
                name: "Balance_CurrencyCode",
                table: "Users",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true); // Temporarily nullable for migration

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "IncomeTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true); // Temporarily nullable for migration

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "ExpenseTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true); // Temporarily nullable for migration

            // Step 2: Migrate data from old columns to new columns
            // Preserve existing currency codes from Balance_Currency and Amount_Currency
            migrationBuilder.Sql(@"
                UPDATE Users
                SET Balance_CurrencyCode = Balance_Currency
                WHERE Balance_Currency IS NOT NULL AND Balance_Currency != '';
            ");

            migrationBuilder.Sql(@"
                UPDATE IncomeTransactions
                SET CurrencyCode = Amount_Currency
                WHERE Amount_Currency IS NOT NULL AND Amount_Currency != '';
            ");

            migrationBuilder.Sql(@"
                UPDATE ExpenseTransactions
                SET CurrencyCode = Amount_Currency
                WHERE Amount_Currency IS NOT NULL AND Amount_Currency != '';
            ");

            // Step 3: Handle any NULL values by using CurrencyPreference as fallback
            // This ensures all records have valid currency codes
            migrationBuilder.Sql(@"
                UPDATE Users
                SET Balance_CurrencyCode = CurrencyPreference
                WHERE Balance_CurrencyCode IS NULL OR Balance_CurrencyCode = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE IncomeTransactions
                SET CurrencyCode = (SELECT CurrencyPreference FROM Users WHERE Id = IncomeTransactions.UserId)
                WHERE CurrencyCode IS NULL OR CurrencyCode = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE ExpenseTransactions
                SET CurrencyCode = (SELECT CurrencyPreference FROM Users WHERE Id = ExpenseTransactions.UserId)
                WHERE CurrencyCode IS NULL OR CurrencyCode = '';
            ");

            // Step 4: Make new columns non-nullable after data migration
            migrationBuilder.AlterColumn<string>(
                name: "Balance_CurrencyCode",
                table: "Users",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "IncomeTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                table: "ExpenseTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            // Step 5: Rename Amount columns
            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "IncomeTransactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "Amount_Amount",
                table: "ExpenseTransactions",
                newName: "Amount");

            // Step 6: Update CurrencyPreference column constraints
            migrationBuilder.AlterColumn<string>(
                name: "CurrencyPreference",
                table: "Users",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Step 7: Drop old currency columns (data already migrated)
            migrationBuilder.DropColumn(
                name: "Balance_Currency",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "Amount_Currency",
                table: "ExpenseTransactions");

            // Step 8: Drop Currencies table (no longer needed)
            migrationBuilder.DropTable(
                name: "Currencies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance_CurrencyCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "ExpenseTransactions");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "IncomeTransactions",
                newName: "Amount_Amount");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "ExpenseTransactions",
                newName: "Amount_Amount");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyPreference",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<string>(
                name: "Balance_Currency",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "IncomeTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Amount_Currency",
                table: "ExpenseTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumericCode = table.Column<int>(type: "int", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsDeleted",
                table: "Currencies",
                column: "IsDeleted");
        }
    }
}
