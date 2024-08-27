namespace BlogApi.Repositories.Interfaces {
    public interface IUnitOfWork {
         PostRepository PostRepository { get; }
         CommentRepository CommentRepository{ get; }
         CategoryRepository CategoryRepository{ get; }
         UserRepository UserRepository{ get;}
         PostReactionRepository PostReactionRepository{ get;  }
         PostTagRepository PostTagRepository{ get;}
         TagRepository TagRepository{ get;}
        Task<int> Complete();
    }
}
