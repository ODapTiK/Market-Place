
namespace UserService
{
    public class ManufacturerRepository : BaseRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(IUserDbContext context) : base(context) { }

        public async Task AddProductAsync(Manufacturer manufacturer, Guid productId, CancellationToken cancellationToken)
        {
            manufacturer.OrganizationProductsId.Add(productId);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveProductAsync(Manufacturer manufactorer, Guid productId, CancellationToken cancellationToken)
        {
            manufactorer.OrganizationProductsId.Remove(productId);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
