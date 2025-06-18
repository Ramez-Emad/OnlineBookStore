using Bulky.DataAccess.Repository.Carts;
using Bulky.DataAccess.Repository.Categories;
using Bulky.DataAccess.Repository.Companies;
using Bulky.DataAccess.Repository.Orders;
using Bulky.DataAccess.Repository.Products;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Bulky.DataAccess.UnitOfWork.UnitOfWork.UnitOfWork
{
    public class UnitOfWork(ApplicationDbContext _dbContext , IConnectionMultiplexer _connectionMultiplexer) : IUnitOfWork
    {

        private IProductRepository? _productRepository;

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_dbContext);
                }
                return _productRepository;
            }
        }


        private ICategoryRepository? _categoryRepository;

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_dbContext);
                }
                return _categoryRepository;
            }
        }

        private ICompanyRepository? _companyRepository;
        public ICompanyRepository CompanyRepository => _companyRepository ??= new CompanyRepository(_dbContext);


        private ICartRepository? _cartRepository;
        public ICartRepository CartRepository => _cartRepository ??= new CartRepository(_connectionMultiplexer);

        private IOrderHeaderRepository? _orderHeaderRepository;
        public IOrderHeaderRepository OrderHeaderRepository => _orderHeaderRepository ??=
            new OrderHeaderRepository(_dbContext);

        private IOrderDetailRepository? _orderDetailRepository;

        public IOrderDetailRepository OrderDetailRepository => _orderDetailRepository ?? new OrderDetailRepository(_dbContext);
        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    }
}
