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
    }
}
