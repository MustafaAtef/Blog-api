using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApi.Entities {
    [Table("Tags")]
    public class Tag {
        public int Id { get; set; }
        public string Name { get; set; }
        public User  User { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<PostTag> PostTags { get; set; }

    }
}
