namespace UserService
{
    public class ReadAdminNotificationUseCase : IReadAdminNotificationUseCase
    {
        private readonly IAdminRepository _repository;

        public ReadAdminNotificationUseCase(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(Guid adminId, Guid notificationId, CancellationToken cancellationToken)
        {
            var admin = await _repository.GetByIdAsync(adminId, cancellationToken)
               ?? throw new EntityNotFoundException(nameof(Admin), adminId);

            admin.AdminNotifications.First(x => x.Id == notificationId).IsRead = true;
            await _repository.UpdateAsync(cancellationToken);
        }
    }
}
