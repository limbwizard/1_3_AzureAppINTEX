// Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using AzureAppINTEX.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Data;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ViewOrders(bool showFraudulent = false, int page = 1, int pageSize = 10)
    {
        var query = _context.Orders
                            .Include(o => o.LineItems)
                            .Include(o => o.Customer)
                            .Where(o => !showFraudulent || (o.Fraud == 1));

        var totalItems = await query.CountAsync();

        var orders = await query.OrderBy(o => o.TransactionID) // Adjust ordering as necessary
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        var viewModel = new AdminOrdersViewModel
        {
            Orders = orders,
            ShowFraudulent = showFraudulent,
            PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalItems
            }
        };

        return View(viewModel);
    }

}
