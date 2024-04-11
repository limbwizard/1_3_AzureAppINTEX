using System.Linq;
using AzureAppINTEX.Models;
using AzureAppINTEX.Data;
namespace AzureAppINTEX.Models
{
    public interface IStoreRepository
    {
        IQueryable<Product> Products { get; }
        IQueryable<Order> GetOrdersByUserId(string userId); // Method to retrieve orders for a specific customer

        void SaveProduct(Product product);
        void CreateProduct(Product product);
        void DeleteProduct(int productId);

        // New method to get product recommendations
        ProductRecommendation GetProductRecommendation(int productId);
    }
}
