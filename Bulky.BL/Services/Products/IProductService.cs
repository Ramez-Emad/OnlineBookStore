using Bulky.BL.Models.Categories;
using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Products
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        public Task<ProductDetailsDto?> GetProductByIdAsync(int? id);
        public Task<int> CreateProductAsync(UpsertProductDto prod);
        public Task<int> UpdateProductAsync(UpsertProductDto prod);
        public Task<bool> DeleteProductAsync(int? id);
    }
}
