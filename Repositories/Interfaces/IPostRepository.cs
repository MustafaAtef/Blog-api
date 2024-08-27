using BlogApi.Dtos;
using BlogApi.Dtos.Post;
using BlogApi.Entities;
using System.Linq.Expressions;

namespace BlogApi.Repositories.Interfaces {
    public interface IPostRepository : IRepository<Post> {
        Task<(int TotalCount, List<CompactPostDto> compactPosts)> GetPaged(SelectionOptions selectionOptions);
        Task<Post?> GetWithComments(Expression<Func<Post, bool>> predicate);
        Task<CompletePostDto?> GetComplete(int id);
    }
}
