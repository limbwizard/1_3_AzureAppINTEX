using AzureAppINTEX.Models.ViewModels; // Adjust the namespace if your PagingInfo is located elsewhere

namespace AzureAppINTEX.ViewModels
{
    public class UserListViewModel
    {
        public IEnumerable<CustomerViewModel> Users { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
