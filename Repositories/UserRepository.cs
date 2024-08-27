using BlogApi.Database;
using BlogApi.Entities;

namespace BlogApi.Repositories {
    public class UserRepository : Repository<User> {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

    }
}
