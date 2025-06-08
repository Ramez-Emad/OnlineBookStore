using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepositories
{
    public interface IRepository<T> where T: class
    {
        public IEnumerable<T> GetAll(Expression<Func<T, object>>? includeProperty = null);
        T? Get(Expression<Func<T, bool>> filter , Expression<Func<T, object>>? includeProperty = null);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemovaRange(IEnumerable<T> entities);

    }
}
