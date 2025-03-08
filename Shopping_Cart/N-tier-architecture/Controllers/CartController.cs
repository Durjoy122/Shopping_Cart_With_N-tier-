using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace N_tire_architecture.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductService _productService;
        private static Dictionary<int, (Product Product, int Quantity)> cart = new();

        public CartController(ProductService productService)
        {
            _productService = productService;
        }

        // Display the cart page
        public IActionResult Index()
        {
            var cartList = cart.Values.ToList();
            return View(cartList);
        }

        // Add product to cart (Update quantity if already exists)
        public IActionResult AddToCart(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            if (cart.ContainsKey(id))
            {
                var existingItem = cart[id];
                cart[id] = (existingItem.Product, existingItem.Quantity + 1); // Increase quantity
            }
            else
            {
                cart[id] = (product, 1); // Add new product with quantity 1
            }

            return RedirectToAction("Index");
        }

        // Remove product from cart
        public IActionResult RemoveFromCart(int id)
        {
            if (cart.ContainsKey(id))
            {
                var existingItem = cart[id];

                if (existingItem.Quantity > 1)
                {
                    // Decrease quantity
                    cart[id] = (existingItem.Product, existingItem.Quantity - 1);
                }
                else
                {
                    // Remove product if quantity is 1
                    cart.Remove(id);
                }
            }

            return RedirectToAction("Index");
        }

        // Clear the entire cart
        public IActionResult ClearCart()
        {
            cart.Clear();
            return RedirectToAction("Index");
        }
    }
}