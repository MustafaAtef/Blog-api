using BlogApi.Dtos;
using BlogApi.Dtos.Category;
using BlogApi.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApi.Controllers {
    [Route("api/categories")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto) {
            return await _categoryService.CreateCategory(createCategoryDto);
        }

        [HttpPut]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(UpdateCategoryDto updateCategoryDto) {
            return await _categoryService.UpdateCategory(updateCategoryDto);
        }

        [HttpGet]
        public async Task<ActionResult<PageList<CategoryDto>>> GetAllCategories([FromQuery]SelectionOptions selectionOptions) {
            return await _categoryService.GetAllCategories(selectionOptions);
        }
    }
}
