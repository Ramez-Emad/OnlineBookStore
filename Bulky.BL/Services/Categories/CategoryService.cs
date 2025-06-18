using AutoMapper;
using Bulky.BL.Models.Categories;
using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Exceptions;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Categories
{
    public class CategoryService(IUnitOfWork _unitOfWork) : ICategoryService
    {
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            var categoriesDTO = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            });

            return categoriesDTO;
        }

        public async Task<int> CreateCategoryAsync(CategoryDTO cat)
        {
            if (cat == null)
                throw new BadRequestException(["Category data is required."]);

            var category = new Category()
            {
                Name = cat.Name,
                Description = cat.Description,
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int? id)
        {
            if (id == null)
                throw new BadRequestException(["id is required."]);


            var cat = await _unitOfWork.CategoryRepository.GetByIdAsync(id.Value);

            if (cat == null)
                throw new CategoryNotFoundException(id.Value);


            return new CategoryDTO
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
            };
        }

        public async Task<int> UpdateCategoryAsync(CategoryDTO cat)
        {
            if (cat == null)
                throw new BadRequestException(["Category data is required."]);

            var categoryFromDB =await  _unitOfWork.CategoryRepository.GetByIdAsync(cat.Id);
            if (categoryFromDB == null)
                throw new CategoryNotFoundException(cat.Id);


            categoryFromDB.Name = cat.Name;
            categoryFromDB.Description = cat.Description;

            _unitOfWork.CategoryRepository.Update(categoryFromDB);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteCategoryAsync(int? id)
        {
            if (id == null)
                throw new BadRequestException(["id is required."]);

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id.Value);

            if (category == null)
                throw new CategoryNotFoundException(id.Value);

            _unitOfWork.CategoryRepository.Delete(category);
            return await _unitOfWork.SaveChangesAsync() > 0;

        }

       
    }
}
