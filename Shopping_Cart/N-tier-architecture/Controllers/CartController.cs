using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using DinkToPdf.Contracts;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using Org.BouncyCastle.Crypto; // Import Bouncy Castle
using System.IO;

namespace N_tire_architecture.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductService _productService;
        private readonly IConverter _converter;
        private static Dictionary<int, (Product Product, int Quantity)> cart = new();

        public CartController(ProductService productService, IConverter converter)
        {
            _productService = productService;
            _converter = converter;
        }

        // Display the cart page
        public IActionResult Index()
        {
            var cartList = cart.Values.ToList();
            decimal totalPrice = cartList.Sum(item => item.Product.Price * item.Quantity);

            // Apply 5% discount if total price exceeds 2000 Taka
            decimal discount = totalPrice > 2000 ? totalPrice * 0.05m : 0;
            decimal finalPrice = totalPrice - discount;

            ViewData["TotalPrice"] = totalPrice;
            ViewData["Discount"] = discount;
            ViewData["FinalPrice"] = finalPrice;

            return View(cartList);
        }

        // Add product to cart
        public IActionResult AddToCart(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            decimal finalPrice = product.Price;
            if (product.Price >= 1000)
            {
                finalPrice = product.Price * 0.8m;
            }
            else if (id % 2 == 0)
            {
                finalPrice = product.Price * 0.5m;
            }

            var discountedProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = (int)finalPrice
            };

            if (cart.ContainsKey(id))
            {
                var existingItem = cart[id];
                cart[id] = (existingItem.Product, existingItem.Quantity + 1);
            }
            else
            {
                cart[id] = (discountedProduct, 1);
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
                    cart[id] = (existingItem.Product, existingItem.Quantity - 1);
                }
                else
                {
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

        // Generate and download PDF of cart items
        public IActionResult DownloadCartPdf()
        {
            var cartList = cart.Values.ToList();
            if (!cartList.Any())
            {
                return Content("Your cart is empty.");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph("Shopping Cart Summary").SetFontSize(20));

                Table table = new Table(5);
                table.AddHeaderCell("ID");
                table.AddHeaderCell("Name");
                table.AddHeaderCell("Unit Price");
                table.AddHeaderCell("Quantity");
                table.AddHeaderCell("Total");

                decimal totalPrice = 0;

                foreach (var item in cartList)
                {
                    decimal total = item.Product.Price * item.Quantity;
                    totalPrice += total;

                    table.AddCell(item.Product.Id.ToString());
                    table.AddCell(item.Product.Name);
                    table.AddCell(item.Product.Price.ToString("F2"));
                    table.AddCell(item.Quantity.ToString());
                    table.AddCell(total.ToString("F2"));
                }

                document.Add(table);
                document.Add(new Paragraph($"Total Price: {totalPrice:F2}").SetFontSize(14));

                document.Close();

                return File(stream.ToArray(), "application/pdf", "CartSummary.pdf");
            }
        }
    }
}