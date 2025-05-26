using Microsoft.EntityFrameworkCore;

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
        public async Task<List<Manufacturer>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Manufacturers.Where(x => true).ToListAsync(cancellationToken);
        }

        public async Task AddNotificationAsync(Manufacturer manufacturer, Notification notification, CancellationToken cancellationToken)
        {
            manufacturer.ManufacturerNotifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public override async Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Manufacturers.Include(x => x.ManufacturerNotifications).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
