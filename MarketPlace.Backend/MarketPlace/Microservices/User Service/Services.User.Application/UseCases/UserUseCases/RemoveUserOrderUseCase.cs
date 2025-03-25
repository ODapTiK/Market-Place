namespace UserService
{
    public class RemoveUserOrderUseCase : IRemoveUserOrderUseCase
    {
        private readonly IUserRepository _userRepository;

        public RemoveUserOrderUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Execute(Guid userId, Guid orderId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                throw new FluentValidation.ValidationException("User Id must not be empty");
            else if (orderId == Guid.Empty)
                throw new FluentValidation.ValidationException("Order Id must not be empty");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            if (!user.UserOrdersId.Contains(orderId))
                throw new EntityNotFoundException("Order", orderId);

            await _userRepository.RemoveOrderAsync(user, orderId, cancellationToken);
        }
    }
}
