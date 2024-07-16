using BlogApi.Dtos;
using BlogApi.Dtos.Post;

namespace BlogApi.ServiceContracts
{
    public interface IPostService
    {
        Task<CompletePostDto> CreatePost(CreatePostDto createPostDto);
        Task<CompletePostDto> UpdatePost(UpdatePostDto updatePostDto);
        Task<PageList<CompactPostDto>> GetAllPosts(SelectionOptions selectionOptions);
        Task<CompletePostDto> GetPostById(int id); 

    }
}
