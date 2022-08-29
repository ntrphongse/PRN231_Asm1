using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(int categoryId);
        Task<Category> AddCategoryAsync(Category newCategory);
        Task<Category> UpdateCategoryAsync(Category updatedCategory);
        Task DeleteCategoryAsync(int categoryId);
    }
}
