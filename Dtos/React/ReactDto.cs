using BlogApi.Dtos.User;
using BlogApi.Entities;

namespace BlogApi.Dtos.React {
    public class ReactDto {
        public ReactiontType reactiontType { get; set; }
        public CreatedByUserDto ReactedBy { get; set; }
        public DateTime ReactedAt { get; set; }
    }
}
