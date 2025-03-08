using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CartService
    {
        private readonly List<Product> _cartItems = new List<Product>();

        public IEnumerable<Product> GetCartItems()
        {
            return _cartItems;
        }

        public void AddToCart(Product product)
        {
            _cartItems.Add(product);
        }

        public void RemoveFromCart(Product product)
        {
            _cartItems.Remove(product);
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }
    }
}
