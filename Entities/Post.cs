namespace BlogApi.Entities {
    public class Post {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int TotalReactions { get; set; }
        public int TotalComments{ get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModification { get; set; }
        public User User { get; set; }
        public int CreatedBy { get; set; }
        public  Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<PostReaction> PostReactions { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostTag> PostTags { get; set; }


    }
}
