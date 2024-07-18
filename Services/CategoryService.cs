using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BlogApi.Services {
    public class CategoryService : ICategoryService {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
        
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto) {
            if (await _isUnique(createCategoryDto.Name) == false) throw new UniqueEntityException("category", "name");
            var category = new Category {
                CreatedBy = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Name = createCategoryDto.Name
            };
            _appDbContext.Set<Category>().Add(category);
            await _appDbContext.SaveChangesAsync();
            return new CategoryDto {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<PageList<CategoryDto>> GetAllCategories(SelectionOptions selectionOptions) {
            IQueryable<Category> categories = _appDbContext.Set<Category>();

            if (selectionOptions.CanFilter("name")) {
                categories = categories.Where(category => category.Name.StartsWith(selectionOptions.FilterValue!));
            }

            Expression<Func<Category, object>> keySelector = category => category.Id;
            if (selectionOptions.OrderBy.Equals("name", StringComparison.CurrentCultureIgnoreCase)) {
                keySelector = category => category.Name;
            }

            if (selectionOptions.Order.Equals("desc", StringComparison.CurrentCultureIgnoreCase))
                    categories = categories.OrderByDescending(keySelector);
            else categories = categories.OrderBy(keySelector);

            var totalCount = await categories.CountAsync();

            categories = categories.Skip((selectionOptions.Page - 1) * selectionOptions.PageSize).Take(selectionOptions.PageSize);

            var result = await categories.Select(category => new CategoryDto { Id = category.Id, Name = category.Name }).ToListAsync();

            return new PageList<CategoryDto>(result, selectionOptions.Page, selectionOptions.PageSize, totalCount);
        }

        public async Task<CategoryDto> UpdateCategory(UpdateCategoryDto updateCategoryDto) {
            var category = await _appDbContext.Set<Category>().SingleOrDefaultAsync(p => p.Id == updateCategoryDto.Id);
            if (category is null) throw new BadRequestException("There isn't a category with the provided id!");
            category.Name = updateCategoryDto.Name;
            await _appDbContext.SaveChangesAsync();
            return new CategoryDto {
                Id = category.Id,
                Name = category.Name
            };
        }
        private async Task<bool> _isUnique(string name) {
            var category = await _appDbContext.Set<Category>().SingleOrDefaultAsync(p => p.Name == name);
            return category == null;
        }
    }
}
