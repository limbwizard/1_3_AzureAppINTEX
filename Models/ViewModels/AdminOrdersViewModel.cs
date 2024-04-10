// ViewModels/AdminOrdersViewModel.cs
namespace AzureAppINTEX.Models.ViewModels
{
    public class AdminOrdersViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public bool ShowFraudulent { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }

}
