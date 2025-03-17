namespace UserService
{
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Execute(Guid userId)
        {
            if (userId.Equals(Guid.Empty))
                throw new FluentValidation.ValidationException("User Id must not be empty");

            var user = await _userRepository.GetByIdAsync(userId, CancellationToken.None);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            await _userRepository.DeleteAsync(user, CancellationToken.None);
        }
    }
}
