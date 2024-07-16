using BlogApi.Dtos.Tag;
using BlogApi.Dtos.User;

namespace BlogApi.Dtos.Post
{
    public class CompactPostDto {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int TotalReactions { get; set; }
        public int TotalComments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModification { get; set; }
        public CreatedByUserDto? CreatedBy { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public IEnumerable<TagDto>? Tags { get; set; }

    }
}
