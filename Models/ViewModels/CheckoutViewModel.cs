using AzureAppINTEX.Models;

namespace AzureAppINTEX.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
        // Add any other properties here that you might need for the checkout process.
        // For example, customer details or shipping information if not already part of the Cart.
    }
}
