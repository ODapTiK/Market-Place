using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderService
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id).IsUnique();
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.OrderDateTime).IsRequired();
            builder.Property(x => x.TotalPrice).IsRequired();

            builder.HasMany(x => x.OrderPoints)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.Orderid);
        }
    }
}
