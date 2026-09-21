using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZHamaster.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Videos",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Videos");
        }
    }
}
