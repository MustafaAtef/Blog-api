namespace BlogApi.Entities {
    public class Category {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public int CreatedBy { get; set; }
        public ICollection<Post> Posts { get; set; }

    }
}