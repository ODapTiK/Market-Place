using MediatR;
using Proto.AuthUser;

namespace AuthorizationService
{
    public class DeleteUserUseCase : IRequestHandler<DeleteUserRequest>
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthUserService.AuthUserServiceClient _userServiceClient;

        public DeleteUserUseCase(IUserRepository userRepository, AuthUserService.AuthUserServiceClient client)
        {
            _userRepository = userRepository;
            _userServiceClient = client;
        }  

        public async Task Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            if (request.Id.Equals(Guid.Empty))
                throw new FluentValidation.ValidationException("User Id must not be empty!");

            var user = await _userRepository.FindByIdAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(User), request.Id);

            var deleteEntityRpcRequest = new DeleteEntityRequest
            {
                Id = user.Id.ToString(),
                Role = (await _userRepository.GetUserRoleAsync(user)).First()
            };

            await _userRepository.DeleteAsync(user, cancellationToken);

            var rpcResponse = await _userServiceClient.DeleteEntityAsync(deleteEntityRpcRequest);

            if(!rpcResponse.Success)
                throw new GRPCRequestFailException(rpcResponse.Message);
        }
    }
}
