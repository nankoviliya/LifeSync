using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Balance_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Balance_CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CurrencyPreference = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpenseType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            var utcTimeNow = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Name", "Code", "CreatedAt", "UpdatedAt", "IsDeleted", "DeletedAt" },
                values: new object[,]
                {
                    { 
                        new Guid("A0B10001-1A2B-4C3D-9E10-000000000001"), 
                        "Bulgarian", 
                        "bg", 
                        utcTimeNow, 
                        utcTimeNow, 
                        false, 
                        null 
                    },
                    { 
                        new Guid("A0B10002-1A2B-4C3D-9E10-000000000002"), 
                        "English", 
                        "en", 
                        utcTimeNow, 
                        utcTimeNow, 
                        false, 
                        null 
                    },
                    { 
                        new Guid("A0B10003-1A2B-4C3D-9E10-000000000003"), 
                        "Ukrainian", 
                        "uk", 
                        utcTimeNow, 
                        utcTimeNow, 
                        false, 
                        null 
                    },
                    { 
                        new Guid("A0B10004-1A2B-4C3D-9E10-000000000004"), 
                        "Russian", 
                        "ru", 
                        utcTimeNow, 
                        utcTimeNow, 
                        false, 
                        null 
                    }
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_CreatedAt",
                table: "ExpenseTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_IsDeleted",
                table: "ExpenseTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_IsDeleted_CreatedAt",
                table: "ExpenseTransactions",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_UserId",
                table: "ExpenseTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_CreatedAt",
                table: "IncomeTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_IsDeleted",
                table: "IncomeTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_IsDeleted_CreatedAt",
                table: "IncomeTransactions",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_UserId",
                table: "IncomeTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDeleted",
                table: "Languages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LanguageId",
                table: "Users",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseTransactions");

            migrationBuilder.DropTable(
                name: "IncomeTransactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
