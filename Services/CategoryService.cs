using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.Repositories.Interfaces;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BlogApi.Services {
    public class CategoryService : ICategoryService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
        
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto) {
            if (await _isUnique(createCategoryDto.Name) == false) throw new UniqueEntityException("category", "name");
            var category = new Category {
                CreatedBy = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Name = createCategoryDto.Name
            };
            _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.Complete();
            return new CategoryDto {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<PageList<CategoryDto>> GetAllCategories(SelectionOptions selectionOptions) {
            var result = await _unitOfWork.CategoryRepository.GetPaged(selectionOptions);
            return new PageList<CategoryDto>(result.PagedData, selectionOptions.Page, selectionOptions.PageSize, result.totalCount);
        }

        public async Task<CategoryDto> UpdateCategory(UpdateCategoryDto updateCategoryDto) {
            var category = await _unitOfWork.CategoryRepository.Get(p => p.Id == updateCategoryDto.Id);
            if (category is null) throw new BadRequestException("There isn't a category with the provided id!");
            category.Name = updateCategoryDto.Name;
            await _unitOfWork.Complete();
            return new CategoryDto {
                Id = category.Id,
                Name = category.Name
            };
        }
        private async Task<bool> _isUnique(string name) {
            var category = await _unitOfWork.CategoryRepository.Get(category => category.Name == name);
            return category == null;
        }
    }
}
