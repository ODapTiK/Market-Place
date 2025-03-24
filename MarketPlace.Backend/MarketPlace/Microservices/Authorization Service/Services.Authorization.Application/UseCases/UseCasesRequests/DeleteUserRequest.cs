using MediatR;

namespace AuthorizationService
{
    public class DeleteUserRequest : IRequest
    {
        public Guid Id { get; set; }
    }
}
