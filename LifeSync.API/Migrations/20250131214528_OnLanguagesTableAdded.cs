using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Migrations;

/// <inheritdoc />
public partial class OnLanguagesTableAdded : Migration
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
                Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Languages", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "Languages",
            columns: new[] { "Id", "Name", "Code" },
            values: new object[,]
            {
                { new Guid("DC8A7CEF-545F-4276-817A-3053B4D8C072"), "English", "en" },
                { new Guid("F2EC0887-DE8F-4BB5-BBE3-199A35394D5E"), "Bulgarian", "bg" },
                { new Guid("DFE7BAE1-1E2B-45BD-92B7-D4057B454F9A"), "Ukrainian", "uk" },
                { new Guid("E3456F2A-66B9-483E-80D8-E107654A71F3"), "Russian", "ru" }
            });

        migrationBuilder.AddColumn<Guid>(
            name: "LanguageId",
            table: "Users",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("DC8A7CEF-545F-4276-817A-3053B4D8C072"));

        migrationBuilder.CreateIndex(
            name: "IX_Users_LanguageId",
            table: "Users",
            column: "LanguageId");

        migrationBuilder.AddForeignKey(
            name: "FK_Users_Languages_LanguageId",
            table: "Users",
            column: "LanguageId",
            principalTable: "Languages",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Users_Languages_LanguageId",
            table: "Users");

        migrationBuilder.DropTable(
            name: "Languages");

        migrationBuilder.DropIndex(
            name: "IX_Users_LanguageId",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "LanguageId",
            table: "Users");
    }
}
