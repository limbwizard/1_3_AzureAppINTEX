using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using AzureAppINTEX.Infrastructure;
using System.Linq;

namespace AzureAppINTEX.Controllers
{
    public class CartController : Controller
    {
        private IStoreRepository repository;

        public CartController(IStoreRepository repo)
        {
            repository = repo;
        }

        public RedirectToActionResult AddToCart(int productId, int quantity = 1)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                Cart cart = SessionCart.GetCart(HttpContext.RequestServices);
                cart.AddItem(product, quantity);
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("ProductsList", "Home");
        }

        public RedirectToActionResult RemoveFromCart(int productId)
        {
            Cart cart = SessionCart.GetCart(HttpContext.RequestServices);
            cart.RemoveItem(productId);
            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index"); // Assumes this is the cart view
        }

        public ViewResult Index()
        {
            var cart = SessionCart.GetCart(HttpContext.RequestServices);
            return View(cart);
        }
    }
}
