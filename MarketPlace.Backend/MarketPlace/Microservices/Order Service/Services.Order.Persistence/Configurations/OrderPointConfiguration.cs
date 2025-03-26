

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderService
{
    public class OrderPointConfiguration : IEntityTypeConfiguration<OrderPoint>
    {
        public void Configure(EntityTypeBuilder<OrderPoint> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.OrderId).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.ProductId).IsRequired().ValueGeneratedNever();
            builder.Property(x => x.NumberOfUnits).IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(x => x.OrderPoints)
                .HasForeignKey(x => x.OrderId);
        }
    }
}
