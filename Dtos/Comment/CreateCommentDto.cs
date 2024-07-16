using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Comment {
    public class CreateCommentDto {
        [BindNever]
        public int? PostId { get; set; }

        [Required(ErrorMessage ="Content must have content!")]
        [MinLength(10, ErrorMessage ="Content length must be greater than 10 characters!")]
        public string? Content { get; set; }
    }
}
