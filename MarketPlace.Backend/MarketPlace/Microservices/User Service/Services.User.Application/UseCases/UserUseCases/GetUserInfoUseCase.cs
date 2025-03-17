namespace UserService
{
    public class GetUserInfoUseCase : IGetUserInfoUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserInfoUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Execute(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new FluentValidation.ValidationException("User Id must not be empty");
            
            var user = await _userRepository.GetByIdAsync(userId, CancellationToken.None);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            return user;
        }
    }
}
