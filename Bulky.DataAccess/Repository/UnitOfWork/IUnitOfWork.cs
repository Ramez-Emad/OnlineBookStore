using Bulky.DataAccess.Repository.Carts;
using Bulky.DataAccess.Repository.Categories;
using Bulky.DataAccess.Repository.Companies;
using Bulky.DataAccess.Repository.Orders;
using Bulky.DataAccess.Repository.Products;
using Bulky.DataAccess.Repository.Users;

namespace Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork
{
    public interface IUnitOfWork
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        ICartRepository CartRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }
        IUserRepository UserRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
