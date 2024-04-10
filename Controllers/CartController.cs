using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using System.Linq;
using AzureAppINTEX.ViewModels;

namespace AzureAppINTEX.Controllers
{
    public class CartController : Controller
    {
        private IStoreRepository repository;

        public CartController(IStoreRepository repo)
        {
            repository = repo;
        }

        public IActionResult AddToCart(int productId, int quantity = 1, string returnUrl = "")
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                Cart cart = SessionCart.GetCart(HttpContext.RequestServices);
                cart.AddItem(product, quantity);
                // Session update is handled within the SessionCart class
            }
            // Redirects to the provided returnUrl if not empty, otherwise defaults to the home index.
            return !string.IsNullOrEmpty(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveFromCart(int productId, string returnUrl = "")
        {
            Cart cart = SessionCart.GetCart(HttpContext.RequestServices);
            cart.RemoveItem(productId);
            // Session update is handled within the SessionCart class
            // Redirects to the cart index, potentially including a returnUrl for further navigation.
            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = SessionCart.GetCart(HttpContext.RequestServices),
                ReturnUrl = returnUrl ?? "/"
            });
        }
    }
}
