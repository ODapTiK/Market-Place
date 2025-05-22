namespace UserService
{
    public class AddUserNotificationUseCase : IAddUserNotificationUseCase
    {
        private readonly IUserRepository _repository;

        public AddUserNotificationUseCase(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(Guid userId, Notification notification, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(userId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(User), userId);

            await _repository.AddNotificationAsync(user, notification, cancellationToken);
        }
    }
}
