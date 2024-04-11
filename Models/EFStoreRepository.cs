// In EFStoreRepository.cs
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Models;
using AzureAppINTEX.Data;

namespace AzureAppINTEX.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private ApplicationDbContext _context;

        public EFStoreRepository(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Product> Products => _context.Products;

        public void CreateProduct(Product product)
        {
            _context.Add(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                _context.Entry(product).State = EntityState.Modified;
            }
            _context.SaveChanges();
        }

        // Implementing the new method
        public ProductRecommendation GetProductRecommendation(int productId)
        {
            // Fetches the first or default ProductRecommendation for the given productId
            return _context.ProductRecommendations.FirstOrDefault(pr => pr.ProductId == productId);
        }
    }
}
