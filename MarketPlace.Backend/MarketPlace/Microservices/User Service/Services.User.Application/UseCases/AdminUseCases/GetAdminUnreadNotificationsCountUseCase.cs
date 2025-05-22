namespace UserService
{
    public class GetAdminUnreadNotificationsCountUseCase : IGetAdminUnreadNotificationsCountUseCase
    {
        private readonly IAdminRepository _repository;

        public GetAdminUnreadNotificationsCountUseCase(IAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Execute(Guid adminId, CancellationToken cancellationToken)
        {
            var admin = await _repository.GetByIdAsync(adminId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Admin), adminId);

            return admin.AdminNotifications.Where(x => x.IsRead == false).ToList().Count();
        }
    }
}
