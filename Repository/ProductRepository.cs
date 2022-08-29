using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        public async Task<Product> AddProductAsync(Product newProduct)
                => await ProductDAO.Instance.AddProductAsync(newProduct);

        public async Task DeleteProductAsync(int productId)
                => await ProductDAO.Instance.DeleteProductAsync(productId);

        public async Task<Product> GetProductAsync(int productId)
                => await ProductDAO.Instance.GetProductAsync(productId);

        public async Task<Product> GetProductAsync(string productName)
                => await ProductDAO.Instance.GetProductAsync(productName);

        public async Task<IEnumerable<Product>> GetProductsAsync()
                => await ProductDAO.Instance.GetProductsAsync();

        public async Task<IEnumerable<Product>> GetProductsAsync(int categoryId)
                => await ProductDAO.Instance.GetProductsAsync(categoryId);

        public async Task<IEnumerable<Product>> SearchProductAsync(string productName)
                => await ProductDAO.Instance.SearchProductAsync(productName);

        public async Task<IEnumerable<Product>> SearchProductAsync(decimal startPrice, decimal endPrice)
                => await ProductDAO.Instance.SearchProductAsync(startPrice, endPrice);

        public async Task<Product> UpdateProductAsync(Product updatedProduct)
                => await ProductDAO.Instance.UpdateProductAsync(updatedProduct);
    }
}
