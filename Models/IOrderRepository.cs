using System.Linq;
using AzureAppINTEX.Models;
using AzureAppINTEX.Data;

namespace AzureAppINTEX.Models
{
    public interface IOrderRepository
    {
        IQueryable<Order> Orders { get; }
        void SaveOrder(Order order);
    }
}
