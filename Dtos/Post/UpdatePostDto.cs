using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Post {
    public class UpdatePostDto {
        public int Id { get; set; }

        [MinLength(10, ErrorMessage = "Post title length must be at least 10 characters!")]
        public string? Title { get; set; }

        [MinLength(10, ErrorMessage = "Post Content length must be at least 10 characters!")]
        public string? Content { get; set; }

        public int? CategoryId { get; set; }
    }
}
