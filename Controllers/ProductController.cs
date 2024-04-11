using Microsoft.AspNetCore.Mvc;
using AzureAppINTEX.Models;
using System.Linq;
using AzureAppINTEX.Models.ViewModels;

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

            // Fetch related product recommendations
            var recommendation = _repository.GetProductRecommendation(id);
            var relatedProductIds = new List<int>();

            if (recommendation != null)
            {
                // Only try to access recommendation properties if recommendation is not null
                relatedProductIds = new[] { recommendation.Rec1, recommendation.Rec2, recommendation.Rec3, recommendation.Rec4, recommendation.Rec5, recommendation.Rec6, recommendation.Rec7, recommendation.Rec8, recommendation.Rec9, recommendation.Rec10 }
                                        .Where(pid => pid.HasValue)
                                        .Select(pid => pid.Value)
                                        .ToList();
            }

            var relatedProducts = _repository.Products
                                             .Where(p => relatedProductIds.Contains(p.ProductID))
                                             .ToList();

            // Constructing ViewModel
            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }
    }
}
