using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeSync.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeviceTrackingAndCsrf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceInfo",
                table: "RefreshTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceInfo",
                table: "RefreshTokens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
