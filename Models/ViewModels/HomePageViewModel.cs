namespace AzureAppINTEX.Models.ViewModels
{
    public class HomePageViewModel
    {
        public Product MostPopularProduct { get; set; }
        public IEnumerable<Product> OtherPopularProducts { get; set; }
        public List<Product> RecommendedProducts { get; set; }
    }

}
