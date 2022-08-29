using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> GetProductsAsync(int categoryId);
        Task<Product> GetProductAsync(int productId);
        Task<Product> GetProductAsync(string productName);
        Task<Product> AddProductAsync(Product newProduct);
        Task<Product> UpdateProductAsync(Product updatedProduct);
        Task DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> SearchProductAsync(string productName);
        Task<IEnumerable<Product>> SearchProductAsync(decimal startPrice, decimal endPrice);
    }
}
