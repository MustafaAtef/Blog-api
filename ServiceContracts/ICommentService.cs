using BlogApi.Dtos.Comment;

namespace BlogApi.ServiceContracts {
    public interface ICommentService {
        Task<CommentDto> CreateComment(CreateCommentDto createCommentDto);
        Task<CommentDto> UpdateComment(UpdateCommentDto updateCommentDto);

    }
}
