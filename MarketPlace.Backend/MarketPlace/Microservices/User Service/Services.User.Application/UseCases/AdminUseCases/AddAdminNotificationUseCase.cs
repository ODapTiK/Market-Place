namespace UserService
{
    public class AddAdminNotificationUseCase : IAddAdminNotificationUseCase
    {
        private readonly IAdminRepository _repository;

        public AddAdminNotificationUseCase(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(Guid adminId, Notification notification, CancellationToken cancellationToken)
        {
            var admin = await _repository.GetByIdAsync(adminId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Admin), adminId);

            await _repository.AddNotificationAsync(admin, notification, cancellationToken);
        }
    }
}
