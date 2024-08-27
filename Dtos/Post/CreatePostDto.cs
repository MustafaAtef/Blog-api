using BlogApi.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Post {
    public class CreatePostDto {

        [Required(ErrorMessage ="Post title is required!")]
        [MinLength(10, ErrorMessage ="Post title length must be at least 10 characters!")]
        public string?Title { get; set; }

        [Required(ErrorMessage = "Post Content is required!")]
        [MinLength(10, ErrorMessage = "Post Content length must be at least 10 characters!")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Category id is required!. Post must belongs to certain category")]
        public int? CategoryId { get; set; }

        [MinListElements(1, ErrorMessage ="Post must have at least one tag!")]
        public List<int>? Tags { get; set; }
    }

}
