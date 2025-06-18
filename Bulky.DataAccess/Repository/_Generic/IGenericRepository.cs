using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository._Generic
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

    }
}
