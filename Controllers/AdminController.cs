using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Data;
using AzureAppINTEX.Models;
using AzureAppINTEX.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using AzureAppINTEX.ViewModels;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<Customer> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
    {
        ViewData["CurrentFilter"] = searchString;

        var usersQuery = _userManager.Users.AsQueryable();
        if (!string.IsNullOrEmpty(searchString))
        {
            usersQuery = usersQuery.Where(u => u.UserName.Contains(searchString));
        }

        var totalUsersCount = await usersQuery.CountAsync();

        // Instead of directly converting to CustomerViewModel, first, get the users
        var users = await usersQuery.OrderBy(u => u.UserName)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

        // Prepare a list to hold the view models including roles
        var viewModels = new List<CustomerViewModel>();
        foreach (var user in users)
        {
            // Fetch roles for each user
            var userRoles = await _userManager.GetRolesAsync(user);
            viewModels.Add(new CustomerViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles.ToList() // Convert IList<string> to List<string> here
            });
        }

        var viewModel = new UserListViewModel
        {
            Users = viewModels,
            PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalUsersCount
            }
        };

        return View("ViewUsers", viewModel); // Ensure the view name matches your actual view file
    }



    public async Task<IActionResult> ViewOrders(bool showFraudulent = false, int page = 1, int pageSize = 10)
    {
        var query = _context.Orders
                            .Include(o => o.LineItems)
                            .Include(o => o.Customer)
                            .Where(o => !showFraudulent || (o.Fraud == 1));

        var totalItems = await query.CountAsync();

        var orders = await query.OrderBy(o => o.TransactionID)
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

    [HttpPost]
    public async Task<IActionResult> AddUser(string userName, string email, string password)
    {
        if (ModelState.IsValid)
        {
            var user = new Customer { UserName = userName, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            // If we're here, something went wrong
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        // Use TempData to pass errors back if redirecting
        TempData["Errors"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        // Redirect back to the Index action to refresh the user list
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _context.Orders
                                  .Include(o => o.Customer)
                                  .Include(o => o.LineItems)
                                  .FirstOrDefaultAsync(o => o.TransactionID == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
    [HttpPost]
    public async Task<IActionResult> DeleteOrder(int transactionId)
    {
        var order = await _context.Orders.FindAsync(transactionId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Order deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Order not found.";
        }
        return RedirectToAction(nameof(ViewOrders));
    }


}
