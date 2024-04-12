/*//Controller********************************************************************************

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureAppINTEX.Data;
using AzureAppINTEX.Models;
using AzureAppINTEX.ViewModels; // Make sure this namespace includes your ViewModel classes
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AzureAppINTEX.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Printing;
using System.Drawing;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly InferenceSession _session;
    private readonly string _onnxModelPath;

    public AdminController(ApplicationDbContext context, UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;

        try
        {
            _session = new InferenceSession("C:\\Users\\kutek\\Documents\\School\\Winter 2024\\INTEX II\\xoxo.onnx");
        }
        catch (Exception ex)
        {
        }
    }

    public async Task<IActionResult> Index(string searchString, int page = 1, int pageSize = 10)
    {
        ViewData["CurrentFilter"] = searchString;
        var usersQuery = _userManager.Users;

        if (!string.IsNullOrEmpty(searchString))
        {
            usersQuery = usersQuery.Where(u => u.UserName.Contains(searchString));
        }

        var totalUsersCount = await usersQuery.CountAsync();
        var users = await usersQuery.OrderBy(u => u.Id) // Order by User Id
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

        var viewModels = new List<CustomerViewModel>();
        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            viewModels.Add(new CustomerViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CustomerID = user.CustomerID,
                BirthDate = user.BirthDate,
                CountryOfResidence = user.CountryOfResidence,
                Gender = user.Gender,
                Age = user.Age,
                Roles = userRoles.ToList()
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

        return View("ViewUsers", viewModel); // Ensure you have a ViewUsers.cshtml view
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.ToListAsync();

        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            CountryOfResidence = user.CountryOfResidence,
            Gender = user.Gender,
            Age = user.Age,
            Roles = userRoles.ToList(),
            AllRoles = allRoles.Select(r => r.Name).ToList(),
            SelectedRoles = userRoles.ToList()
        };

        return View(model); // Ensure you have an EditUser.cshtml view
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.UserName = model.UserName;
            user.Email = model.Email;
            // Add other properties like FirstName, LastName, etc.

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();
            var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();

            await _userManager.AddToRolesAsync(user, rolesToAdd);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        model.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); // Refresh roles in case of error
        return View(model);
    }





    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ViewOrders(bool showFraudulent = false, int page = 1, int pageSize = 10)
    {
        var query = _context.Orders
                            .Include(o => o.LineItems)
                            .Include(o => o.Customer)
                            .Where(o => !showFraudulent || (o.Fraud == 1));

        var totalItems = await query.CountAsync();

        var orders = await query.OrderByDescending(o => o.Date)
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


    [HttpGet]
    public IActionResult AddUser()
    {
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
        var model = new AddUserViewModel
        {
            AllRoles = allRoles
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new Customer
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                CountryOfResidence = model.CountryOfResidence,
                Gender = model.Gender,
                Age = model.Age
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (model.SelectedRoles != null && model.SelectedRoles.Count > 0)
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

                return RedirectToAction("Index"); // Ensure this redirects to your intended destination
            }
            else
            {
                // If creation failed, re-prepare the roles for the form
                model.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        // If we got this far, something failed, redisplay form
        model.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList(); // Ensure roles are populated on failed validation
        return View(model);
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
                                      .ThenInclude(li => li.Product)
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
    // Display all products
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Products()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    // Show add/edit product form
    public IActionResult EditProduct(int? id)
    {
        if (id == null)
        {
            return View(new Product());
        }
        else
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductID == id);
            return View(product);
        }
    }

    // Save product changes
    [HttpPost]
    public async Task<IActionResult> EditProduct(Product product)
    {
        if (ModelState.IsValid)
        {
            if (product.ProductID == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                _context.Products.Update(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }
        return View(product);
    }

    // Delete a product
    [HttpPost]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Products));
    }





    public IActionResult ReviewOrders()
    {
        var records = _context.Orders
            .OrderByDescending(o => o.Date)
            .Take(20)
            .ToList();  // Fetch the 20 most recent records
        var predictions = new List<FraudPrediction>();  // Your ViewModel for the view
        // Dictionary mapping the numeric prediction to an animal type
        var class_type_dict = new Dictionary<int, string>
            {
                { 0, "Not Fraud" },
                { 1, "Fraud" }
            };
        foreach (var record in records)
        {
            // Calculate days since January 1, 2022
            var january1_2022 = new DateTime(2022, 1, 1);
            var daysSinceJan12022 = record.Date.HasValue ? Math.Abs((record.Date.Value - january1_2022).Days) : 0;
            // Preprocess features to make them compatible with the model
            var input = new List<float>
                {
*//*                    (float)record.,
*//*                    (float)record.Time,    
                    // fix amount if it's null
                    (float)(record.Amount),
                    // fix date
                    daysSinceJan12022,
                    // Check the Dummy Coded Data
                    record.DayOfWeek == "Mon" ? 1 : 0,
                    record.DayOfWeek == "Sat" ? 1 : 0,
                    record.DayOfWeek == "Sun" ? 1 : 0,
                    record.DayOfWeek == "Thu" ? 1 : 0,
                    record.DayOfWeek == "Tue" ? 1 : 0,
                    record.DayOfWeek == "Wed" ? 1 : 0,
                    record.EntryMode == "Pin" ? 1 : 0,
                    record.EntryMode == "Tap" ? 1 : 0,
                    record.TypeOfTransaction == "Online" ? 1 : 0,
                    record.TypeOfTransaction == "POS" ? 1 : 0,
                    record.CountryOfTransaction == "India" ? 1 : 0,
                    record.CountryOfTransaction == "Russia" ? 1 : 0,
                    record.CountryOfTransaction == "USA" ? 1 : 0,
                    record.CountryOfTransaction == "UnitedKingdom" ? 1 : 0,
                    // Use CountryOfTransaction if ShippingAddress is null
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "India" ? 1 : 0,
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "Russia" ? 1 : 0,
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "USA" ? 1 : 0,
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "UnitedKingdom" ? 1 : 0,
                    record.Bank == "HSBC" ? 1 : 0,
                    record.Bank == "Halifax" ? 1 : 0,
                    record.Bank == "Lloyds" ? 1 : 0,
                    record.Bank == "Metro" ? 1 : 0,
                    record.Bank == "Monzo" ? 1 : 0,
                    record.Bank == "RBS" ? 1 : 0,

                    record.TypeOfCard == "Visa" ? 1 : 0
*//*                    1,1,1,1,1,1,1,1,1,1,1,1,1
*//*
                };
            var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });
            var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };
            string predictionResult;
            using (var results = _session.Run(inputs))
            {
                var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                predictionResult = prediction != null && prediction.Length > 0 ? class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown") : "Error in prediction";
            }
            predictions.Add(new FraudPrediction { Order = record, Prediction = predictionResult });
        }
        return View(predictions);
    }

}
















*/