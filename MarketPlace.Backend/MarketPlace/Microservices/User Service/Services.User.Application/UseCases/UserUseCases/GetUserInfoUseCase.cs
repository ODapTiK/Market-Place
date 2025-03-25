namespace UserService
{
    public class GetUserInfoUseCase : IGetUserInfoUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserInfoUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Execute(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                throw new FluentValidation.ValidationException("User Id must not be empty");
            
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            return user;
        }
    }
}
