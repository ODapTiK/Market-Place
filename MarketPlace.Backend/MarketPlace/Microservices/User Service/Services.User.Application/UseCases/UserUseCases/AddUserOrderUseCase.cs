namespace UserService
{
    public class AddUserOrderUseCase : IAddUserOrderUseCase
    {
        private readonly IUserRepository _userRepository;

        public AddUserOrderUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Execute(Guid userId, Guid orderId)
        {
            if (userId == Guid.Empty)
                throw new FluentValidation.ValidationException("User Id must not be empty");
            else if (orderId == Guid.Empty)
                throw new FluentValidation.ValidationException("Order Id must not be empty");

            var user = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            await _userRepository.AddOrderAsync(user, orderId, CancellationToken.None);
        }
    }
}
