using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCountryColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Locations");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_City_Zip",
                table: "Locations",
                columns: new[] { "City", "Zip" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_City_Zip",
                table: "Locations");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Locations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "The Country where the event is located");
        }
    }
}
