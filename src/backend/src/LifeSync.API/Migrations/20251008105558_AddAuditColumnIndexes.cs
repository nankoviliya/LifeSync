using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_CreatedAt",
                table: "IncomeTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_IsDeleted_CreatedAt",
                table: "IncomeTransactions",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_CreatedAt",
                table: "ExpenseTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_IsDeleted_CreatedAt",
                table: "ExpenseTransactions",
                columns: new[] { "IsDeleted", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IncomeTransactions_CreatedAt",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IncomeTransactions_IsDeleted_CreatedAt",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTransactions_CreatedAt",
                table: "ExpenseTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTransactions_IsDeleted_CreatedAt",
                table: "ExpenseTransactions");
        }
    }
}
