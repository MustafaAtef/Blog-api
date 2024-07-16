namespace BlogApi.Entities {
    public class PostReaction {
        public User User { get; set; }
        public int UserId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public Reaction Reaction { get; set; }
        public int ReactionId { get; set; }
        public DateTime ReactedAt { get; set; }

    }
}
