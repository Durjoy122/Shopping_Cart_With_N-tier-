using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;

namespace N_tire_architecture.Controllers
{
   
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult Search (string name)
        {
            var product = _productService.GetProductByName(name);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Prices = new List<int> {100, 500, 200, 300, 1000, 700 };
            return View();  // Ensure this exists!
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Prices = new List<int> {100, 500, 200, 300, 1000, 700 };
                return View(product);
            }

            try
            {
                _productService.AddProduct(product);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while adding the product.");
                ViewBag.Prices = new List<int> {100, 500, 200, 300, 1000, 700 };
                return View(product);
            }
        }

        public IActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Prices = new List<int> {100,  500, 200, 300, 1000, 700 };
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Prices = new List<int> {100, 500, 200, 300, 1000, 700 };
                return View(product);
            }

            _productService.UpdateProduct(product);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }

        // Search products by name
        public IActionResult Search_Name(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewData["Message"] = "Please enter a search term.";
                return View(new List<Product>());
            }

            var products = _productService.SearchProductsByName(query);
            if (!products.Any())
            {
                ViewData["Message"] = "No products found.";
            }

            return View(products);
        }
    }
}