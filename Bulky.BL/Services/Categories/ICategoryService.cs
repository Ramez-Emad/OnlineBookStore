using Bulky.BL.Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Categories
{
    public interface ICategoryService
    {
        public Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        public Task<CategoryDTO> GetCategoryByIdAsync(int? id);
        public Task<int> UpdateCategoryAsync(CategoryDTO cat);
        public Task<bool> DeleteCategoryAsync(int? id);
        public Task<int> CreateCategoryAsync(CategoryDTO cat);

    }
}
