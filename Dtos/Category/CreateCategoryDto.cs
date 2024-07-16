using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Category {
    public class CreateCategoryDto {

        [Required(ErrorMessage = "Category name is required!")]
        [Length(3, 50, ErrorMessage ="Category name length must be between 3 and 50 characters!")]
        public string? Name { get; set; }

    }
}
