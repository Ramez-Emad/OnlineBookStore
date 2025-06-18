using AutoMapper;
using Bulky.BL.Models.Categories;
using Bulky.BL.Models.Products;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Exceptions;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;

namespace Bulky.BL.Services.Products
{
    internal class ProductService(IUnitOfWork _unitOfWork, IMapper _mapper) : IProductService
    {
        public async Task<int> CreateProductAsync(UpsertProductDto prod)
        {
            if (prod == null)
                throw new BadRequestException(["Product data is required."]);

            var product = _mapper.Map<Product>(prod);

            await _unitOfWork.ProductRepository.AddAsync(product);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var Products = await _unitOfWork.ProductRepository.GetAllAsync(includes:prod => prod.Category);
            var ProductsDTO = _mapper.Map<IEnumerable<ProductDTO>>(Products);
            return ProductsDTO;
        }

        public async Task<ProductDetailsDto?> GetProductByIdAsync(int? id)
        {
            if (id == null)
                throw new BadRequestException(["id is required."]);


            var prod = await _unitOfWork.ProductRepository.GetByIdAsync(id.Value , includes: prod => prod.Category);

            if (prod == null)
                throw new ProductNotFoundException(id.Value);

            return _mapper.Map<ProductDetailsDto>(prod);
        }

        public async Task<int> UpdateProductAsync(UpsertProductDto prod)
        {
            if (prod == null)
                throw new BadRequestException(["Product data is required."]);

            var productFromDB = await _unitOfWork.ProductRepository.GetByIdAsync(prod.Id);

            if (productFromDB == null)
                throw new ProductNotFoundException(prod.Id);

            _mapper.Map(prod , productFromDB);

            _unitOfWork.ProductRepository.Update(productFromDB);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteProductAsync(int? id)
        {
            if (id == null)
                throw new BadRequestException(["id is required."]);

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(id.Value);

            if (product == null)
                throw new ProductNotFoundException(id.Value);

            _unitOfWork.ProductRepository.Delete(product);
            return await _unitOfWork.SaveChangesAsync() > 0;

        }
    }
}
