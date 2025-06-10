using Bulky.DataAccess.Repository.IRepositories;
using Bulky.Models;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderDetailRepository(ApplicationDbContext _db) : Repository<OrderDetail>(_db) , IOrderDetailRepository
    {
    
    }
}
