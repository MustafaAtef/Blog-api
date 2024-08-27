using BlogApi.Entities;
using System.Linq.Expressions;

namespace BlogApi.Repositories.Interfaces {
    public interface ICommentRepository : IRepository<Comment> {
        Task<Comment?> GetWithPost(Expression<Func<Comment, bool>> predicate);
    }
}
