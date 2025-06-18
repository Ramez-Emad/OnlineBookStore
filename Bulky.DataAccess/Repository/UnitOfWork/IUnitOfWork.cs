using Bulky.DataAccess.Repository.Carts;
using Bulky.DataAccess.Repository.Categories;
using Bulky.DataAccess.Repository.Companies;
using Bulky.DataAccess.Repository.Orders;
using Bulky.DataAccess.Repository.Products;

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

        Task<int> SaveChangesAsync();
    }
}
