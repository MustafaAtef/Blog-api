using BlogApi.Dtos;
using BlogApi.Dtos.React;
using BlogApi.Entities;

namespace BlogApi.Repositories.Interfaces {
    public interface IPostReactionRepository : IRepository<PostReaction> {
        Task<List<ReactDto>> GetPaged(int postId, SelectionOptions selectionOptions);
    }
}
