using BusinessObject;
using eStoreLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        private static ProductDAO instance = null;
        private static readonly object instanceLock = new object();
        private ProductDAO()
        {

        }
        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var database = new FStoreContext();
            return await database.Products
                .Include(pro => pro.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int categoryId)
        {
            var database = new FStoreContext();
            return await database.Products
                .Where(pro => pro.CategoryId == categoryId)
                .Include(pro => pro.Category)
                .ToListAsync();
        }

        private async Task<int> GetNextProductIdAsync()
        {
            var database = new FStoreContext();
            return await database.Products.MaxAsync(pro => pro.ProductId) + 1;
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            var database = new FStoreContext();
            return await database.Products
                .Include(pro => pro.Category)
                .SingleOrDefaultAsync(product => product.ProductId == productId);
        }

        public async Task<Product> GetProductAsync(string productName)
        {
            var database = new FStoreContext();
            if (string.IsNullOrEmpty(productName))
            {
                throw new ApplicationException("Search keyword is empty!!");
            }
            return await database.Products
                .Include(pro => pro.Category)
                .SingleOrDefaultAsync(product => product.ProductName.ToLower().Equals(productName.ToLower()));
        }

        public async Task<Product> AddProductAsync(Product newProduct)
        {
            await CheckProduct(newProduct);
            var database = new FStoreContext();
            newProduct.ProductId = await GetNextProductIdAsync();
            await database.Products.AddAsync(newProduct);
            await database.SaveChangesAsync();
            return newProduct;
        }

        public async Task<Product> UpdateProductAsync(Product updatedProduct)
        {
            if (await GetProductAsync(updatedProduct.ProductId) == null)
            {
                throw new Exception($"Product with the ID {updatedProduct.ProductId} does not exist! " +
                    $"Please check with the developer for more information");
            }
            await CheckProduct(updatedProduct);
            var database = new FStoreContext();
            database.Products.Update(updatedProduct);
            await database.SaveChangesAsync();
            return updatedProduct;
        }

        public async Task DeleteProductAsync(int productId)
        {
            Product deletedProduct = await GetProductAsync(productId);
            if (deletedProduct == null)
            {
                throw new Exception($"Product with the ID {productId} does not exist...");
            }
            var database = new FStoreContext();
            database.Products.Remove(deletedProduct);
            await database.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(string productName)
        {
            var database = new FStoreContext();
            if (string.IsNullOrEmpty(productName))
            {
                throw new ApplicationException("Search keyword is empty!!");
            }
            return await database.Products
                .Include(pro => pro.Category)
                .Where(product => product.ProductName.ToLower().Contains(productName.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductAsync(decimal startPrice, decimal endPrice)
        {
            if (startPrice > endPrice)
            {
                decimal temp = startPrice;
                startPrice = endPrice;
                endPrice = temp;
            }
            var database = new FStoreContext();
            return await database.Products
                .Include(pro => pro.Category)
                .Where(product => product.UnitPrice >= startPrice && product.UnitPrice <= endPrice)
                .ToListAsync();
        }

        private async Task CheckProduct(Product product)
        {
            var database = new FStoreContext();
            if (await database.Categories.FindAsync(product.CategoryId) == null)
            {
                throw new ApplicationException("Category is not existed!!");
            }
            product.ProductName.StringValidate(allowEmpty: false,
                emptyErrorMessage: "Product Name cannot be empty!!",
                minLength: 2, minLengthErrorMessage: "Product Name needs to be at least 2 characters!!",
                maxLength: 40, maxLengthErrorMessage: "Product Name is limited to 40 characters!!");

            product.Weight.StringValidate(allowEmpty: false,
                emptyErrorMessage: "Product Weight cannot be empty!!",
                minLength: 2, minLengthErrorMessage: "Product Weight needs to be at least 2 characters!!",
                maxLength: 20, maxLengthErrorMessage: "Product Weight is limited to 20 characters!!");

            product.UnitPrice.DecimalValidate(minimum: 0,
                        minErrorMessage: "Product Unit Price has to be a positive number!!",
                        maximum: decimal.MaxValue,
                        maxErrorMessage: $"Product Unit Price is limited to the value of {decimal.MaxValue}!");

            product.UnitsInStock.IntegerValidate(minimum: 0,
                        minErrorMessage: "Product Unit in Stock has to be a positive number!!",
                        maximum: int.MaxValue,
                        maxErrorMessage: $"Product Unit in Stock is limited to the value of {int.MaxValue}!");
        }
    }
}
