using MediatR;

namespace AuthorizationService
{
    public class DeleteUserUseCase : IRequestHandler<DeleteUserRequest>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }  

        public async Task Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            if (request.Id.Equals(Guid.Empty))
                throw new FluentValidation.ValidationException("User Id must not be empty!");

            var user = await _userRepository.FindByIdAsync(request.Id, CancellationToken.None)
                ?? throw new EntityNotFoundException(nameof(User), request.Id);

            await _userRepository.DeleteAsync(user, CancellationToken.None);
        }
    }
}
