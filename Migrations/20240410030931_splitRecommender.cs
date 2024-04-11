using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzureAppINTEX.Migrations
{
    /// <inheritdoc />
    public partial class splitRecommender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.CreateTable(
                name: "CustomerRecommendations",
                columns: table => new
                {
                    CustomerRecommendationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rec1 = table.Column<int>(type: "int", nullable: true),
                    Rec2 = table.Column<int>(type: "int", nullable: true),
                    Rec3 = table.Column<int>(type: "int", nullable: true),
                    Rec4 = table.Column<int>(type: "int", nullable: true),
                    Rec5 = table.Column<int>(type: "int", nullable: true),
                    Rec6 = table.Column<int>(type: "int", nullable: true),
                    Rec7 = table.Column<int>(type: "int", nullable: true),
                    Rec8 = table.Column<int>(type: "int", nullable: true),
                    Rec9 = table.Column<int>(type: "int", nullable: true),
                    Rec10 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRecommendations", x => x.CustomerRecommendationId);
                    table.ForeignKey(
                        name: "FK_CustomerRecommendations_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductRecommendations",
                columns: table => new
                {
                    ProductRecommendationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Rec1 = table.Column<int>(type: "int", nullable: true),
                    Rec2 = table.Column<int>(type: "int", nullable: true),
                    Rec3 = table.Column<int>(type: "int", nullable: true),
                    Rec4 = table.Column<int>(type: "int", nullable: true),
                    Rec5 = table.Column<int>(type: "int", nullable: true),
                    Rec6 = table.Column<int>(type: "int", nullable: true),
                    Rec7 = table.Column<int>(type: "int", nullable: true),
                    Rec8 = table.Column<int>(type: "int", nullable: true),
                    Rec9 = table.Column<int>(type: "int", nullable: true),
                    Rec10 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecommendations", x => x.ProductRecommendationId);
                    table.ForeignKey(
                        name: "FK_ProductRecommendations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRecommendations_CustomerId",
                table: "CustomerRecommendations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecommendations_ProductId",
                table: "ProductRecommendations",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerRecommendations");

            migrationBuilder.DropTable(
                name: "ProductRecommendations");

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    RecommendationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Rec1 = table.Column<int>(type: "int", nullable: true),
                    Rec10 = table.Column<int>(type: "int", nullable: true),
                    Rec2 = table.Column<int>(type: "int", nullable: true),
                    Rec3 = table.Column<int>(type: "int", nullable: true),
                    Rec4 = table.Column<int>(type: "int", nullable: true),
                    Rec5 = table.Column<int>(type: "int", nullable: true),
                    Rec6 = table.Column<int>(type: "int", nullable: true),
                    Rec7 = table.Column<int>(type: "int", nullable: true),
                    Rec8 = table.Column<int>(type: "int", nullable: true),
                    Rec9 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.RecommendationId);
                    table.ForeignKey(
                        name: "FK_Recommendations_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recommendations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_CustomerId",
                table: "Recommendations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_ProductId",
                table: "Recommendations",
                column: "ProductId");
        }
    }
}
