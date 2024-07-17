namespace BlogApi.Entities {
    public class PostReaction {
        public User User { get; set; }
        public int UserId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public ReactiontType reactiontType { get; set; }
        public DateTime ReactedAt { get; set; }

    }

    public enum ReactiontType { 
        LIKE = 0,
        LOVE,
        CARE,
        WOW,
        ANGRY
    }
}
