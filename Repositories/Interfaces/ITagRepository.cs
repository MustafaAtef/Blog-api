using BlogApi.Dtos.Tag;
using BlogApi.Entities;
using System.Linq.Expressions;

namespace BlogApi.Repositories.Interfaces {
    public interface ITagRepository : IRepository<Tag> {
        Task<IList<TagDto>> Get(List<int> ids);
        Task BulkDelete(Expression<Func<PostTag, bool>> predicate);
    }
}
