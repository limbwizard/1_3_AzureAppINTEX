using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAppINTEX.Migrations
{
    /// <inheritdoc />
    public partial class reallylastonetho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_AspNetUsers_CustomerId",
                table: "Recommendations");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Recommendations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_AspNetUsers_CustomerId",
                table: "Recommendations",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recommendations_AspNetUsers_CustomerId",
                table: "Recommendations");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Recommendations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Recommendations_AspNetUsers_CustomerId",
                table: "Recommendations",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
