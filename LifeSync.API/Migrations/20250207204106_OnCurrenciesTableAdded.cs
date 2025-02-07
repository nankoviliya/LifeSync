using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Migrations
{
    /// <inheritdoc />
    public partial class OnCurrenciesTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NumericCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "Name", "NativeName", "Symbol", "NumericCode" },
                values: new object[,]
                {
                    // US Dollar
                    { new Guid("2089BA96-1FC3-425C-82EE-9D832B557A1A"), "USD", "US Dollar", "US Dollar", "$", 840 },
        
                    // Euro
                    { new Guid("2189BA96-1FC3-425C-82EE-9D832B557A1A"), "EUR", "Euro", "Euro", "€", 978 },
        
                    // Bulgarian Lev (native name in Bulgarian)
                    { new Guid("2289BA96-1FC3-425C-82EE-9D832B557A1A"), "BGN", "Bulgarian Lev", "Български лев", "лв", 975 },
        
                    // Ukrainian Hryvnia (native name in Ukrainian)
                    { new Guid("2389BA96-1FC3-425C-82EE-9D832B557A1A"), "UAH", "Ukrainian Hryvnia", "Українська гривня", "₴", 980 },
    
                    // Russian Ruble (native name in Russian)
                    { new Guid("2489BA96-1FC3-425C-82EE-9D832B557A1A"), "RUB", "Russian Ruble", "Российский рубль", "₽", 643 },
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
