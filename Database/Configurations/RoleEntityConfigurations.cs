using BlogApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Database.Configurations {
    public class RoleEntityConfigurations : IEntityTypeConfiguration<Role> {
        public void Configure(EntityTypeBuilder<Role> builder) {
            builder.ToTable("Roles");
            builder.HasData(new Role { Id = 1, Name = "User" },
                new Role { Id = 2, Name = "Writer" },
                new Role { Id = 3, Name = "Admin" }
            );
        }
    }
}
