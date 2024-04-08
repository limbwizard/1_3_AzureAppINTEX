using System.Linq;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Models;
using AzureAppINTEX.Data;

namespace AzureAppINTEX.Models
{
    public class EFOrderRepository : IOrderRepository
    {
        private ApplicationDbContext _context;

        public EFOrderRepository(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public IQueryable<Order> Orders => _context.Orders.Include(o => o.LineItems).ThenInclude(li => li.Product);

        public void SaveOrder(Order order)
        {
            if (order.TransactionID == 0)
            {
                _context.Orders.Add(order);
            }
            else
            {
                _context.Entry(order).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }
    }
}
