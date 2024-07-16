using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Database.Configurations {
    public class PostTagEntityConfigurations : IEntityTypeConfiguration<PostTag> {
        public void Configure(EntityTypeBuilder<PostTag> builder) {
            builder.ToTable("PostsTags");
            builder.HasKey(p => new { p.PostId, p.TagId });
        }
    }
}
