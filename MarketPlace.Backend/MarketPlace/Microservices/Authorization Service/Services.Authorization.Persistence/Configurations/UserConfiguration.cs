using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationService
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.Email).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Password).IsRequired();
        }
    }
}
