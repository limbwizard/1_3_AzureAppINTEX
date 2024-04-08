using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using System.Linq;

namespace AzureAppINTEX.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Cart _cart;

        public OrderController(IOrderRepository orderRepository, Cart cartService)
        {
            _orderRepository = orderRepository;
            _cart = cartService;
        }

        public IActionResult Checkout() => View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            // Checks if the cart is empty
            if (!_cart.Lines.Any())
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
                return View(order);
            }

            if (ModelState.IsValid)
            {
                // Initialize the collection to avoid null reference
                order.LineItems = new List<LineItem>();

                // Populate LineItems from Cart
                foreach (var line in _cart.Lines)
                {
                    order.LineItems.Add(new LineItem
                    {
                        ProductID = line.Product.ProductID, // Assuming LineItem has a ProductID
                        Quantity = line.Quantity ?? 0, // Assuming Quantity is nullable in LineItem
                        Product = line.Product, // Including Product reference if needed
                        Order = order // Establishing the relationship
                    });
                }

                // Assuming you have set up Order to automatically calculate Amount
                // order.Amount = order.LineItems.Sum(li => li.Quantity * li.Product.Price);

                // Save the order and its line items
                _orderRepository.SaveOrder(order);

                // Clear the cart after saving the order
                _cart.Clear();

                // Redirect to a completion page, passing OrderID for confirmation, etc.
                return RedirectToAction("Completed", new { orderId = order.TransactionID });
            }

            // If there are validation issues, return to the view with the current Order object
            return View(order);
        }

        // Example completion action
        public IActionResult Completed(int orderId)
        {
            // Logic to display confirmation, etc.
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
