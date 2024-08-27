using System.Linq.Expressions;

namespace BlogApi.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<TEntity?> Get(int id);
        Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAll();
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
