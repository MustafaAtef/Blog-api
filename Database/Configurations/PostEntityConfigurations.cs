using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Database.Configurations {
    public class PostEntityConfigurations : IEntityTypeConfiguration<Post> {
        public void Configure(EntityTypeBuilder<Post> builder) {
            builder.ToTable("Posts");
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.LastModification).HasDefaultValueSql("GETDATE()");
            builder.Property(p => p.TotalComments).HasDefaultValue(0);
            builder.Property(p => p.TotalReactions).HasDefaultValue(0);

            builder.HasOne(p => p.User).WithMany(p => p.Posts).HasForeignKey(p => p.CreatedBy).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Category).WithMany(p => p.Posts).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
