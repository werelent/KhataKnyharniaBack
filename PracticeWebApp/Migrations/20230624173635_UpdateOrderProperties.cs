using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "BookIds",
                table: "Orders",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookQuantitiesJson",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BookQuantitiesJson",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Orders",
                newName: "BookIds");
        }
    }
}
