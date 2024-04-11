using System.Collections.Generic;

namespace AzureAppINTEX.Models.ViewModels
{
    public class CustomerOrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public PagingInfo PagingInfo { get; set; }
    }
}
