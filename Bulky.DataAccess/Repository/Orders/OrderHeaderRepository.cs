using Bulky.DataAccess.Repository._Generic;
using BulkyWeb.Data;

namespace Bulky.DataAccess.Repository.Orders
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {

        public OrderHeaderRepository(ApplicationDbContext db) : base(db) 
        {
        }
        
    }
}

