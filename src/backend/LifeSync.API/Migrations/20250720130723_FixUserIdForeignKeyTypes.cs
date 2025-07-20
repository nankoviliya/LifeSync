using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Migrations
{
    /// <inheritdoc />
    public partial class FixUserIdForeignKeyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseTransactions_Users_UserId1",
                table: "ExpenseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_Users_UserId1",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IncomeTransactions_UserId1",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTransactions_UserId1",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ExpenseTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "IncomeTransactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ExpenseTransactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_UserId",
                table: "IncomeTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_UserId",
                table: "ExpenseTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseTransactions_Users_UserId",
                table: "ExpenseTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_Users_UserId",
                table: "IncomeTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseTransactions_Users_UserId",
                table: "ExpenseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_Users_UserId",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IncomeTransactions_UserId",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTransactions_UserId",
                table: "ExpenseTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "IncomeTransactions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "IncomeTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ExpenseTransactions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "ExpenseTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_UserId1",
                table: "IncomeTransactions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_UserId1",
                table: "ExpenseTransactions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseTransactions_Users_UserId1",
                table: "ExpenseTransactions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_Users_UserId1",
                table: "IncomeTransactions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
