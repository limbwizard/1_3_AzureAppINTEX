using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using AzureAppINTEX.Infrastructure;
using AzureAppINTEX.Models.ViewModels;

public class CartController : Controller
{
    private readonly IStoreRepository repository;
    private readonly Cart cart;

    public CartController(IStoreRepository repo, Cart cartService)
    {
        repository = repo;
        cart = cartService;
    }

    // Displays the current state of the cart.
    public ViewResult Index(string returnUrl)
    {
        return View(new CartIndexViewModel
        {
            Cart = cart,
            ReturnUrl = returnUrl
        });
    }

    // Adds a product to the cart.
    public RedirectToActionResult AddToCart(int productId, string returnUrl)
    {
        Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);

        if (product != null)
        {
            cart.AddItem(product, 1);
        }
        return RedirectToAction("Index", new { returnUrl });
    }

    // Removes a product from the cart.
    public RedirectToActionResult RemoveFromCart(int productId, string returnUrl)
    {
        cart.RemoveItem(productId);
        return RedirectToAction("Index", new { returnUrl });
    }
}
