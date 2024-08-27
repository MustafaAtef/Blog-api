using BlogApi.Database;
using BlogApi.Entities;

namespace BlogApi.Repositories {
    public class PostTagRepository : Repository<Post> {
        public PostTagRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

    }
}
