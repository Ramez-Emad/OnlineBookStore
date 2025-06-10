using Bulky.DataAccess.Repository.IRepositories;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // Naming convention: Product => ProductRepository
            var repoTypeName = $"{TypeName}Repository";
            var repoType = Assembly.GetExecutingAssembly()
                                   .GetTypes()
                                   .FirstOrDefault(t =>
                                       t.Name.Equals(repoTypeName, StringComparison.OrdinalIgnoreCase) &&
                                       typeof(IRepository<T>).IsAssignableFrom(t));

            IRepository<T> repositoryInstance;

            if (repoType != null)
            {
                repositoryInstance = (IRepository<T>)Activator.CreateInstance(repoType, _dbContext)!;
            }
            else
            {
                repositoryInstance = new Repository<T>(_dbContext);
            }

            _repositories[TypeName] = repositoryInstance;
            return repositoryInstance;
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();

    }
}
