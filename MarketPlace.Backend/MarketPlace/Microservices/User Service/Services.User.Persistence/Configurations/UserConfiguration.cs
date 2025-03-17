using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserService
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Surname).IsRequired().HasMaxLength(150);
            builder.Property(x => x.BirthDate).IsRequired();
        }
    }
}
