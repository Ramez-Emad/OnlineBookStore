using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T>(ApplicationDbContext _db) : IRepository<T> where T : class
    {
        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public T? Get(Expression<Func<T, bool>> filter , Expression<Func<T, object>>? includeProperty = null)
        {
            if (includeProperty == null)
                return _db.Set<T>().FirstOrDefault(filter);
            return _db.Set<T>().Include(includeProperty).FirstOrDefault(filter);

        }

        public IEnumerable<T> GetAll(Expression<Func<T, object>>? includeProperty = null)
        {
            if (includeProperty == null)
                return _db.Set<T>().ToList();

            return _db.Set<T>().Include(includeProperty).ToList();
        }

        public void RemovaRange(IEnumerable<T> entities)
        {
            _db.Set<T>().RemoveRange(entities);
        }

        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _db.Set<T>().Update(entity);
        }
    }
}
