using Proto.OrderUser;

namespace UserService
{
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly OrderUserService.OrderUserServiceClient _orderUserServiceClient;

        public DeleteUserUseCase(IUserRepository userRepository,
                                 OrderUserService.OrderUserServiceClient orderUserServiceClient)
        {
            _userRepository = userRepository;
            _orderUserServiceClient = orderUserServiceClient;
        }

        public async Task Execute(Guid userId, CancellationToken cancellationToken)
        {
            if (userId.Equals(Guid.Empty))
                throw new FluentValidation.ValidationException("User Id must not be empty");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                throw new EntityNotFoundException(nameof(User), userId);

            await _userRepository.DeleteAsync(user, cancellationToken);

            var rpcCartRequest = new CartRequest
            {
                UserId = userId.ToString()
            };

            var rpcOrderRequest = new DeleteUserOrdersRequest
            {
                UserId = userId.ToString()
            };


            var rpcCartUnaryCall = _orderUserServiceClient.DeleteCartAsync(rpcCartRequest);
            var rpcOrderUnaryCall = _orderUserServiceClient.DeleteUserOrdersAsync(rpcOrderRequest);

            Task.WaitAll(rpcCartUnaryCall.ResponseAsync, rpcOrderUnaryCall.ResponseAsync);

            var rpcCartResponse = await rpcCartUnaryCall;
            var rpcOrderResponse = await rpcOrderUnaryCall;

            if(!rpcCartResponse.Success)
                throw new GRPCRequestFailException(rpcCartResponse.Message);
            if(!rpcOrderResponse.Success)
                throw new GRPCRequestFailException(rpcOrderResponse.Message);
        }
    }
}
