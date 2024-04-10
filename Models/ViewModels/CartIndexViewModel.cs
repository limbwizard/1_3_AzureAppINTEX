namespace AzureAppINTEX.Models.ViewModels
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}

