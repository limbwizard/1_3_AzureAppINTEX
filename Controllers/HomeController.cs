using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using AzureAppINTEX.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AzureAppINTEX.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository _repository;
        private readonly UserManager<Customer> _userManager;

        public int PageSize = 20;

        public HomeController(IStoreRepository repository, UserManager<Customer> userManager)
        {
            _repository = repository;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            // Your specified product IDs in order
            var topProductIds = new List<int> { 27, 1, 37, 8, 7, 3, 6, 34, 33, 10 };

            // Fetch the products first
            var topProducts = _repository.Products
                .Where(p => topProductIds.Contains(p.ProductID))
                .ToList() // Execute the query and fetch the data
                .OrderBy(p => topProductIds.IndexOf(p.ProductID)) // Then order the results in memory
                .ToList();

            var mostPopularProduct = topProducts.First();
            var otherPopularProducts = topProducts.Skip(1); // Skip the first one


            var user = await _userManager.GetUserAsync(User);
            List<Product> recommendedProducts = new List<Product>();

            if (user != null)
            {
                var customerRecommendation = _repository.GetCustomerRecommendation(user.Id);
                if (customerRecommendation != null)
                {
                    var recommendedProductIds = new List<int>
            {
                customerRecommendation.Rec1 ?? 0,
                customerRecommendation.Rec2 ?? 0,
                customerRecommendation.Rec3 ?? 0,
                customerRecommendation.Rec4 ?? 0,
                customerRecommendation.Rec5 ?? 0,
                customerRecommendation.Rec6 ?? 0,
                customerRecommendation.Rec7 ?? 0,
                customerRecommendation.Rec8 ?? 0,
                customerRecommendation.Rec9 ?? 0,
                customerRecommendation.Rec10 ?? 0
            };

                    recommendedProducts = _repository.Products
                    .Where(p => recommendedProductIds.Contains(p.ProductID))
                    .Where(p => p.ProductID != 0)
                    .ToList();
                }
            }

            var viewModel = new HomePageViewModel
            {
                MostPopularProduct = mostPopularProduct,
                OtherPopularProducts = otherPopularProducts,
                RecommendedProducts = recommendedProducts
            };

            return View(viewModel);
        }


        private Dictionary<string, string> CategoryMapping = new Dictionary<string, string>
{
    // Updated mappings with new categories from your list
    {"Star Wars", "Star Wars"},
    {"DC", "DC"},
    {"Harry Potter", "Harry Potter"},
    {"Star Wars - Flight", "Star Wars"},
    {"Personal - Vehicle", "Vehicle"},
    {"Car - Vehicle", "Vehicle"},
    {"Flight - Vehicle - Promotional", "Flight"},
    {"Vehicle - Motorbike - Person", "Vehicle"},
    {"Vehicle - Car", "Vehicle"},
    {"Game", "Game"},
    {"Vehicle - Flight", "Vehicle"},
    {"Minifig", "Minifig"},
    {"Promotional - Food", "Promotional"},
    {"Vehicle - Train - Animal", "Vehicle"},
    {"Vehicle - Motorbike", "Vehicle"},
    {"Flight - Building", "Flight"},
    {"Flight - Vehicle", "Flight"},
    {"Part - Energy", "Part"},
    {"Energy - Part", "Energy"},
    {"Part - Train - Vehicle", "Part"},
    {"Part - Vehicle", "Part"},
    {"Part", "Part"},
    {"Character - Disney", "Character"},
    {"Harry Potter - Minifig", "Harry Potter"},
    {"Floral - Colorful", "Floral"},
    {"Colorful - Animal", "Colorful"},
    {"Animal - Minifig", "Minifig"},
    {"Vehicle - Farm", "Vehicle"},
    {"Colorful - Personal", "Colorful"},
    {"Colorful - Structure - Disney", "Colorful"},
    {"Structure", "Structure"},
    {"Character - Vehicle - Flight", "Character"},
    {"Minifig - Structure", "Minifig"},
    {"Minifig - Structure - Colorful", "Minifig"},
    {"Energy", "Energy"},
    {"Harry Potter - Character", "Harry Potter"}
};
        private IEnumerable<string> GetGeneralizedCategories(IEnumerable<string> detailedCategories)
        {
            var generalizedCategories = new HashSet<string>();
            foreach (var category in detailedCategories)
            {
                if (CategoryMapping.TryGetValue(category, out var generalizedCategory))
                {
                    generalizedCategories.Add(generalizedCategory);
                }
                else
                {
                    generalizedCategories.Add(category);
                }
            }
            return generalizedCategories;
        }
        private Dictionary<string, List<string>> GetReverseCategoryMapping()
        {
            var reverseMapping = new Dictionary<string, List<string>>();
            foreach (var pair in CategoryMapping)
            {
                if (!reverseMapping.ContainsKey(pair.Value))
                {
                    reverseMapping.Add(pair.Value, new List<string>());
                }
                reverseMapping[pair.Value].Add(pair.Key);
            }
            return reverseMapping;
        }
        public ViewResult ProductsList(string category, string primaryColor, int productPage = 1, int pageSize = 5)
        {
            var reverseMapping = GetReverseCategoryMapping();
            var productsQuery = _repository.Products.AsQueryable();
            if (!string.IsNullOrEmpty(category))
            {
                if (reverseMapping.ContainsKey(category))
                {
                    var detailedCategories = reverseMapping[category];
                    productsQuery = productsQuery.Where(p => detailedCategories.Contains(p.Category));
                }
                else
                {
                    productsQuery = productsQuery.Where(p => p.Category == category);
                }
            }
            if (!string.IsNullOrEmpty(primaryColor))
            {
                productsQuery = productsQuery.Where(p => p.PrimaryColor == primaryColor);
            }
            var totalItems = productsQuery.Count();
            var viewModel = new ProductsListViewModel
            {
                Products = productsQuery
                    .OrderBy(p => p.ProductID)
                    .Skip((productPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = pageSize,
                    TotalItems = totalItems
                },
                CurrentCategory = category
            };
            ViewBag.PrimaryColors = _repository.Products
                .Select(p => p.PrimaryColor)
                .Distinct()
                .OrderBy(color => color)
                .ToList();
            ViewBag.Categories = GetGeneralizedCategories(_repository.Products
                .Select(p => p.Category)
                .Distinct())
                .OrderBy(category => category)
                .ToList();
            ViewBag.SelectedPrimaryColor = primaryColor;
            ViewBag.SelectedCategory = category;
            return View(viewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = _repository.GetOrdersByUserId(user.Id)
                .OrderByDescending(o => o.Date) // Sorting by Date descending
                .Select(o => new OrderViewModel
                {
                    TransactionID = o.TransactionID,
                    Date = o.Date,
                    Amount = o.Amount,
                    // Inside your controller, when projecting to the OrderViewModel
                    LineItems = o.LineItems.Select(li => new LineItemViewModel
                    {
                        ProductName = li.Product.Name,
                        Quantity = li.Quantity ?? 0, // Provide a default value (e.g., 0) if Quantity is null
                        Price = li.Product.Price
                    })
                }).ToList();
            var viewModel = new CustomerOrdersViewModel
            {
                Orders = orders
            };
            return View(viewModel);
        }
    }
}
