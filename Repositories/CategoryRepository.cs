using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.Entities;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Repositories {
    public class CategoryRepository : Repository<Category>, ICategoryRepository {
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

        public async Task<(int totalCount, List<CategoryDto> PagedData)> GetPaged(SelectionOptions selectionOptions) {
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
            return (totalCount, result);
        }
    }
}
