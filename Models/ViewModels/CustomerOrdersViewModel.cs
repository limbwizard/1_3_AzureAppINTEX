using System.Collections.Generic;
namespace AzureAppINTEX.Models.ViewModels
{
    public class CustomerOrdersViewModel
    {
        public IEnumerable<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public PagingInfo PagingInfo { get; set; }
    }
    // ViewModel for individual orders
    public class OrderViewModel
    {
        public int TransactionID { get; set; }
        public DateTime? Date { get; set; }
        public decimal Amount { get; set; }
        public IEnumerable<LineItemViewModel> LineItems { get; set; } = new List<LineItemViewModel>();
    }
    // ViewModel for line items in an order
    public class LineItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}