
namespace UserService
{
    public class UpdateUserLogoUseCase : IUpdateUserLogoUseCase
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserLogoUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Execute(Guid userId, string base64Image, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(User), userId);

            user.Logo = base64Image;

            await _userRepository.UpdateAsync(cancellationToken);
        }
    }
}
