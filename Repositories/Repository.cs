using BlogApi.Database;
using BlogApi.Database.Configurations;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Repositories {
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: class {
        protected readonly AppDbContext _appDbContext;

        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public void Add(TEntity entity) {
            _appDbContext.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities) {
            _appDbContext.Set<TEntity>().AddRange(entities);
        }

        public async Task<TEntity?> Get(int id) {
            return await _appDbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate) {
            return await _appDbContext.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAll() {
            return await _appDbContext.Set<TEntity>().ToListAsync();
        }

        public void Remove(TEntity entity) {
            _appDbContext.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities) {
            _appDbContext.Set<TEntity>().RemoveRange(entities);
        }
    }
}
