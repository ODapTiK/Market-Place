namespace UserService
{
    public class GetUserUreadNotificationsCountUseCase : IGetUserUnreadNotificationsCountUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserUreadNotificationsCountUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Execute(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(User), userId);

            return user.UserNotifications.Where(x => x.IsRead == false).ToList().Count();   
        }
    }
}
