using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAppINTEX.Migrations
{
    /// <inheritdoc />
    public partial class firxdsf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerInfoID",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CustomerInfoID",
                table: "AspNetUsers",
                newName: "CustomerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerID",
                table: "AspNetUsers",
                newName: "CustomerInfoID");

            migrationBuilder.AddColumn<int>(
                name: "CustomerInfoID",
                table: "Orders",
                type: "int",
                nullable: true);
        }
    }
}
