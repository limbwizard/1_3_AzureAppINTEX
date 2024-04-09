using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAppINTEX.Migrations
{
    /// <inheritdoc />
    public partial class nowfinalprobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderTransactionID",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_OrderTransactionID",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "OrderTransactionID",
                table: "LineItems");

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_TransactionID",
                table: "LineItems",
                column: "TransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_TransactionID",
                table: "LineItems",
                column: "TransactionID",
                principalTable: "Orders",
                principalColumn: "TransactionID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_TransactionID",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_TransactionID",
                table: "LineItems");

            migrationBuilder.AddColumn<int>(
                name: "OrderTransactionID",
                table: "LineItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_OrderTransactionID",
                table: "LineItems",
                column: "OrderTransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderTransactionID",
                table: "LineItems",
                column: "OrderTransactionID",
                principalTable: "Orders",
                principalColumn: "TransactionID");
        }
    }
}
