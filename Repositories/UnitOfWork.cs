using BlogApi.Database;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Repositories {
    public class UnitOfWork : IUnitOfWork {
        private readonly AppDbContext _appDbContext;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            CommentRepository = new(appDbContext);
            CategoryRepository = new(appDbContext);
            UserRepository = new(appDbContext);
            PostReactionRepository= new(appDbContext);
            PostTagRepository= new(appDbContext);
            PostRepository = new(appDbContext);
            TagRepository = new(appDbContext);
        }
        

        public CommentRepository CommentRepository { get; }

        public CategoryRepository CategoryRepository { get; }

        public UserRepository UserRepository { get; }

        public PostReactionRepository PostReactionRepository { get; }

        public PostTagRepository PostTagRepository { get; }

        public PostRepository PostRepository { get; }
        public TagRepository TagRepository{ get; }
        public async Task<int> Complete() {
            return await _appDbContext.SaveChangesAsync();    
        }
    }
}
