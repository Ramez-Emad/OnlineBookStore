using Bulky.DataAccess.Repository.IRepositories;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork(ApplicationDbContext _dbContext) : IUnitOfWork
    {
       

        private readonly Dictionary<string, object> _repositories = [];

        public IRepository<T> GetRepository<T>() where T : class
        {
            var TypeName = typeof(T).Name;

            if (_repositories.ContainsKey(TypeName))
            {
                return (IRepository<T>)_repositories[TypeName];
            }

            var Object = new Repository<T>(_dbContext);

            _repositories[TypeName] = Object;

            return Object;
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    }
}
