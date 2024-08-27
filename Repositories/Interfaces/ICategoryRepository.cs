using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.Entities;

namespace BlogApi.Repositories.Interfaces {
    public interface ICategoryRepository : IRepository<Category> {
        Task<(int totalCount, List<CategoryDto> PagedData)> GetPaged(SelectionOptions selectionOptions);
    }
}
