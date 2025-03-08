using DAL.Models;
using DAL.Repositories;

namespace BLL.Services
{
    public class ProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _productRepository.GetAll();
        }

        public Product GetProductByName(string name)
        {
            return _productRepository.GetByName(name);
        }

        public Product GetProductById(int id)
        {
            return _productRepository.GetById(id);
        }

        public void AddProduct(Product product)
        {
            _productRepository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }
    }
}

