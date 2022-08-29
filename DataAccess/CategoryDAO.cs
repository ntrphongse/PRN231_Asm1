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
    public class CategoryDAO
    {
        private static CategoryDAO instance = null;
        private static readonly object instanceLock = new object();
        private CategoryDAO()
        {

        }

        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                    return instance;
                }
            }
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var database = new FStoreContext();
            return await database.Categories.ToListAsync();
        }

        private async Task<int> GetNextCategoryIdAsync()
        {
            var database = new FStoreContext();
            return await database.Categories.MaxAsync(mem => mem.CategoryId) + 1;
        }

        public async Task<Category> GetCategoryAsync(int categoryId)
        {
            var database = new FStoreContext();
            return await database.Categories.SingleOrDefaultAsync(member => member.CategoryId == categoryId);
        }

        public async Task<Category> AddCategoryAsync(Category newCategory)
        {
            CheckCategory(newCategory);
            var database = new FStoreContext();
            newCategory.CategoryId = await GetNextCategoryIdAsync();
            await database.Categories.AddAsync(newCategory);
            await database.SaveChangesAsync();

            return newCategory;
        }

        public async Task<Category> UpdateCategoryAsync(Category updatedCategory)
        {
            if (await GetCategoryAsync(updatedCategory.CategoryId) == null)
            {
                throw new Exception($"Category with the ID {updatedCategory.CategoryId} does not exist! Please check with the developer for more information");
            }
            CheckCategory(updatedCategory);
            var database = new FStoreContext();
            database.Categories.Update(updatedCategory);
            await database.SaveChangesAsync();
            return updatedCategory;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            Category deletedCategory = await GetCategoryAsync(categoryId);
            if (deletedCategory == null)
            {
                throw new Exception($"Category with the ID {categoryId} does not exist! Please check again...");
            }
            var database = new FStoreContext();
            database.Categories.Remove(deletedCategory);
            await database.SaveChangesAsync();
        }

        private void CheckCategory(Category category)
        {
            category.CategoryName.StringValidate(allowEmpty: false, emptyErrorMessage: "Category Name cannot be empty!!",
                                minLength: 2, minLengthErrorMessage: "Category Name needs to be at least 2 characters!",
                                maxLength: 40, maxLengthErrorMessage: "Category Name is limited to 40 characters!");
        }
    }
}
