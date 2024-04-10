using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
// Include any other necessary namespaces

namespace AzureAppINTEX.Controllers
{
    public class ProductController : Controller
    {
        private readonly IStoreRepository _repository;

        public ProductController(IStoreRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Item(int id)
        {
            var product = _repository.Products.FirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
