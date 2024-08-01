using BlogApi.Dtos;
using BlogApi.Dtos.React;

namespace BlogApi.ServiceContracts {
    public interface IReactService {
        Task<ReactDto> CreateReact(ReqReactDto createReactDto);
        Task<PageList<ReactDto>> GetAllPostReacts(int postId, SelectionOptions selectionOptions);
        Task<ReactDto> DeleteReact(int postId);
    }
}
