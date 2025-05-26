namespace UserService
{
    public interface IAdminRepository : IBaseRepository<Admin>
    {
        public Task<List<Admin>> GetAllAsync(CancellationToken cancellationToken);
        public Task AddNotificationAsync(Admin manufacturer, Notification notification, CancellationToken cancellationToken);
    }
}
