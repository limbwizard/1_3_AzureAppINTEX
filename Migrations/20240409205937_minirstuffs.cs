using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAppINTEX.Migrations
{
    /// <inheritdoc />
    public partial class minirstuffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerID",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CustomerID",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerID",
                table: "Orders",
                newName: "IX_Orders_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "CustomerID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                newName: "IX_Orders_CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerID",
                table: "Orders",
                column: "CustomerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
