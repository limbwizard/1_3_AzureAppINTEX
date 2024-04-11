using System.Linq;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Models;
using AzureAppINTEX.Data;

namespace AzureAppINTEX.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public EFStoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Products => _context.Products;

        public IQueryable<Order> GetOrdersByUserId(string userId)
        {
            return _context.Orders.Where(o => o.UserId == userId);
        }

        public void SaveProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

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
    }
}
