using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BlogApi.Database.Configurations {
    public class UserEntityConfigurations : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("Users");
            builder.HasIndex(p => p.Username).IsUnique();
            builder.HasIndex(p => p.Email).IsUnique();
            builder.Property(p => p.Username).HasColumnType("nvarchar(50)");
            builder.Property(p => p.Password).HasColumnType("nvarchar(70)");
            builder.Property(p => p.Email).HasColumnType("nvarchar(256)");
            builder.Property(p => p.RoleId).HasDefaultValue(1);
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

            //builder.HasMany(p => p.Posts).WithOne(p => p.User);
        }
    }
}
