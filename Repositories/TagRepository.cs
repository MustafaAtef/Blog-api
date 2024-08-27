using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.Dtos.Post;
using BlogApi.Dtos.Tag;
using BlogApi.Entities;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Repositories {
    public class TagRepository  : Repository<Tag>, ITagRepository {
        public TagRepository (AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

        public async Task<IList<TagDto>> Get(List<int> ids) {
            return await _appDbContext.Set<Tag>()
                 .Where(tag => ids.Contains(tag.Id))
                 .Select(tag => new TagDto { Id = tag.Id, Name = tag.Name })
                 .ToListAsync();
        }
        public async Task BulkDelete(Expression<Func<PostTag, bool>> predicate) {
            await _appDbContext.Set<PostTag>().Where(predicate).ExecuteDeleteAsync();
        }
    }
}
