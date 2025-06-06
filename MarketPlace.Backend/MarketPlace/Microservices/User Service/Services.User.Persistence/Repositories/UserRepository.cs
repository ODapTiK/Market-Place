﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UserService
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IUserDbContext context) : base(context) { }

        public async Task AddOrderAsync(User user, Guid orderId, CancellationToken cancellationToken)
        {
            user.UserOrdersId.Add(orderId);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveOrderAsync(User user, Guid orderId, CancellationToken cancellationToken)
        {
            user.UserOrdersId.Remove(orderId);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<User>> GetManyAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Users.Include(x => x.UserNotifications).Where(predicate).ToListAsync(cancellationToken);
        }

        public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users.Include(x => x.UserNotifications).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddNotificationAsync(User user, Notification notification, CancellationToken cancellationToken)
        {
            user.UserNotifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
