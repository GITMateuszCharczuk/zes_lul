using lab_3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using lab_3.Services;

namespace lab_3.Controllers
{
    public class ProductModelController : Controller
    {
        private readonly JsonService<ProductModel> _jsonService;

        public ProductModelController()
        {
            _jsonService = new JsonService<ProductModel>("C:\\Users\\mateu\\Documents\\GitHub\\zes_lul\\lab_3_proj\\lab_3\\lab_3\\Data\\Products.json");
        }

        // Read all ProductModels
        public IActionResult Index(string searchTerm, string category, string sortBy)
        {
            var productModels = _jsonService.ReadAll();

            if (!string.IsNullOrEmpty(searchTerm))
                productModels = productModels.Where(p => p.Name.Contains(searchTerm)).ToList();

            if (!string.IsNullOrEmpty(category))
                productModels = productModels.Where(p => p.Category == category).ToList();

            productModels = sortBy switch
            {
                "Price" => productModels.OrderBy(p => p.Price).ToList(),
                "Name" => productModels.OrderBy(p => p.Name).ToList(),
                _ => productModels
            };

            return View(productModels);
        }

        // Create new ProductModel
        public IActionResult Create()
        { 
            var productModel = new ProductModel();
            return View(productModel);
        }

        [HttpPost]
        public IActionResult Create(ProductModel ProductModel)
        {
            var productModels = _jsonService.ReadAll();
            ProductModel.Id = productModels.Any() ? productModels.Max(p => p.Id) + 1 : 1;
            productModels.Add(ProductModel);
            _jsonService.WriteAll(productModels);
            return RedirectToAction("Index");
        }

        // Edit ProductModel
        public IActionResult Edit(int id)
        {
            var productModel = _jsonService.ReadAll().FirstOrDefault(p => p.Id == id);
            return productModel == null ? NotFound() : (IActionResult)View("Create", productModel);
        }

        [HttpPost]
        public IActionResult Edit(ProductModel updatedProductModel)
        {
            var productModels = _jsonService.ReadAll();
            var productModel = productModels.FirstOrDefault(p => p.Id == updatedProductModel.Id);
            if (productModel == null) return NotFound();

            productModel.Name = updatedProductModel.Name;
            productModel.Category = updatedProductModel.Category;
            productModel.Price = updatedProductModel.Price;
            productModel.IsAvailable = updatedProductModel.IsAvailable;
            _jsonService.WriteAll(productModels);

            return RedirectToAction("Index");
        }

        // Delete ProductModel
        public IActionResult Delete(int id)
        {
            var productModels = _jsonService.ReadAll();
            var productModel = productModels.FirstOrDefault(p => p.Id == id);
            if (productModel == null) return NotFound();

            productModels.Remove(productModel);
            _jsonService.WriteAll(productModels);

            return RedirectToAction("Index");
        }
    }
}