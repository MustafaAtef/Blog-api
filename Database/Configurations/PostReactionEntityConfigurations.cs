using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Database.Configurations {
    public class PostReactionEntityConfigurations : IEntityTypeConfiguration<PostReaction> {
        public void Configure(EntityTypeBuilder<PostReaction> builder) {
            builder.ToTable("PostReactions");
            builder.HasKey(p => new { p.UserId, p.PostId });
            builder.Property(p => p.ReactedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}