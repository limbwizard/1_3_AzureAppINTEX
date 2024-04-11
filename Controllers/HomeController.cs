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

        public IActionResult Index()
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

            var viewModel = new HomePageViewModel
            {
                MostPopularProduct = mostPopularProduct,
                OtherPopularProducts = otherPopularProducts
            };

            return View(viewModel);
        }



        public ViewResult ProductsList(string category, string primaryColor, int productPage = 1, int pageSize = 5)
        {
            var productsQuery = _repository.Products.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
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

            // Prepare filter data
            ViewBag.PrimaryColors = _repository.Products
                .Select(p => p.PrimaryColor)
                .Distinct()
                .OrderBy(color => color)
                .ToList();

            ViewBag.Categories = _repository.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(category => category)
                .ToList();

            // Store selected filter values
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
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // If the user is not authenticated, redirect to login
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Retrieve orders for the current user using the repository method
            var orders = _repository.GetOrdersByUserId(user.Id).ToList();

            // Create a view model to pass orders to the view
            var viewModel = new CustomerOrdersViewModel
            {
                Orders = orders
            };

            return View(viewModel);
        }
    }
}
