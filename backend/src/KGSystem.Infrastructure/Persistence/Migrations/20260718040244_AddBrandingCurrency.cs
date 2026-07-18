using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandingCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "BrandingSettings",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EGP");

            migrationBuilder.Sql("UPDATE BrandingSettings SET Currency = 'EGP' WHERE Currency IS NULL OR Currency = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "BrandingSettings");
        }
    }
}
