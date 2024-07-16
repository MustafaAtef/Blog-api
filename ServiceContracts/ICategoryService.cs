using BlogApi.Dtos;
using BlogApi.Dtos.Category;

namespace BlogApi.ServiceContracts {
    public interface ICategoryService {
        Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategory(UpdateCategoryDto updateCategoryDto);
        Task<PageList<CategoryDto>> GetAllCategories(SelectionOptions selectionOptions);
    }
}
