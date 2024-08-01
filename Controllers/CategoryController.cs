using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers {
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Authorize(Roles = "3")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto) {
            return await _categoryService.CreateCategory(createCategoryDto);
        }

        [HttpPut]
        [Authorize(Roles = "3")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(UpdateCategoryDto updateCategoryDto) {
            return await _categoryService.UpdateCategory(updateCategoryDto);
        }

        [HttpGet]
        public async Task<ActionResult<PageList<CategoryDto>>> GetAllCategories([FromQuery]SelectionOptions selectionOptions) {
            return await _categoryService.GetAllCategories(selectionOptions);
        }
    }
}
