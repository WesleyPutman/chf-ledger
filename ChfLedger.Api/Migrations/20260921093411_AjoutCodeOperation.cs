using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChfLedger.Api.Migrations
{
    /// <inheritdoc />
    public partial class AjoutCodeOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Operations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Operations");
        }
    }
}
