using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<Category> AddCategoryAsync(Category newCategory)
                => await CategoryDAO.Instance.AddCategoryAsync(newCategory);

        public async Task DeleteCategoryAsync(int categoryId)
                => await CategoryDAO.Instance.DeleteCategoryAsync(categoryId);

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
                => await CategoryDAO.Instance.GetCategoriesAsync();

        public async Task<Category> GetCategoryAsync(int categoryId)
                => await CategoryDAO.Instance.GetCategoryAsync(categoryId);

        public async Task<Category> UpdateCategoryAsync(Category updatedCategory)
                => await CategoryDAO.Instance.UpdateCategoryAsync(updatedCategory);
    }
}
