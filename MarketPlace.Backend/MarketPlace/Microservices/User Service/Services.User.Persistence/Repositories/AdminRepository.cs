using Microsoft.EntityFrameworkCore;

namespace UserService
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(IUserDbContext userDbContext) : base(userDbContext) { }

        public async Task<List<Admin>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Admins.Where(x => true).ToListAsync(cancellationToken);
        }

        public async Task AddNotificationAsync(Admin manufacturer, Notification notification, CancellationToken cancellationToken)
        {
            manufacturer.AdminNotifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public override async Task<Admin?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Admins.Include(x => x.AdminNotifications).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
