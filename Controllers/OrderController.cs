using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AzureAppINTEX.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Data;
using Microsoft.AspNetCore.Authorization;

namespace AzureAppINTEX.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Cart _cart;
        private readonly UserManager<Customer> _userManager;

        public OrderController(ApplicationDbContext context, Cart cartService, UserManager<Customer> userManager)
        {
            _context = context;
            _cart = cartService;
            _userManager = userManager;
        }

        public IActionResult Checkout() => View(new Order());

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            if (!_cart.Lines.Any())
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
                return View(order);
            }

            if (ModelState.IsValid)
            {
                order.Date = DateTime.Now; // or DateTime.UtcNow;
                order.UserId = _userManager.GetUserId(User);
                order.LineItems = new List<LineItem>();

                foreach (var item in _cart.Lines)
                {
                    var product = await _context.Products.FindAsync(item.Product.ProductID);
                    if (product != null)
                    {
                        var lineItem = new LineItem
                        {
                            ProductID = item.Product.ProductID,
                            Quantity = item.Quantity ?? 0,
                            // Product and Price are not directly set here, they are navigated via ProductID
                        };
                        order.LineItems.Add(lineItem);
                    }
                }

                order.Amount = order.LineItems.Sum(li => (li.Quantity ?? 0) * (_context.Products.Find(li.ProductID)?.Price ?? 0));
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                _cart.Clear();
                return RedirectToAction("Completed", new { orderId = order.TransactionID });
            }

            return View(order);
        }

        public IActionResult Completed(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.TransactionID == orderId);
            if (order == null || (User.Identity.IsAuthenticated && order.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
