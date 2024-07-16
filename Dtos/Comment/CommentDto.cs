using BlogApi.Dtos.User;

namespace BlogApi.Dtos.Comment
{
    public class CommentDto {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public CreatedByUserDto? CommentedBy { get; set; }

    }
}
