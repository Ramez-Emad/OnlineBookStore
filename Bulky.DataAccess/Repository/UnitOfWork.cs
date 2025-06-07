using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ICategoryRepository CategoryRepository { get; private set; }

        public IProductRepository ProductRepository { get; private set; }

        public ICompanyRepository CompanyRepository { get; private set; }

        public ICartRepository CartRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db , IConnectionMultiplexer connectionMultiplexer)
        {
            CategoryRepository = new CategoryRepository(db);
            ProductRepository = new ProductRepository(db);
            CompanyRepository = new CompanyRepository(db);
            CartRepository = new CartRepository(connectionMultiplexer);
            _db = db;
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
