using Bulky.DataAccess.Repository._Generic;
using BulkyWeb.Data;

namespace Bulky.DataAccess.Repository.Orders
{
    public class OrderDetailRepository(ApplicationDbContext _db) : GenericRepository<OrderDetail>(_db) , IOrderDetailRepository
    {
    
    }
}
