using BlogApi.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Database.Configurations {
    public class CategoryEntityConfigurations : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.ToTable("Categories");
            builder.HasIndex(p => p.Name).IsUnique();
            builder.Property(p => p.Name).HasColumnType("nvarchar(50)");
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.HasMany(p => p.Posts).WithOne(p => p.Category).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.User).WithMany(p => p.Categories).HasForeignKey(p => p.CreatedBy).OnDelete(DeleteBehavior.Restrict);

        }
    }
}