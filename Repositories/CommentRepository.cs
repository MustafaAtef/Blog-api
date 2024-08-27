using BlogApi.Database;
using BlogApi.Entities;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Repositories {
    public class CommentRepository : Repository<Comment>, ICommentRepository {
        public CommentRepository(AppDbContext appDbContext)  :base(appDbContext)
        {
            
        }

        public async Task<Comment?> GetWithPost(Expression<Func<Comment, bool>> predicate) {
            return await _appDbContext.Set<Comment>().Include(c => c.Post).SingleOrDefaultAsync(predicate);
        }
    }
}
