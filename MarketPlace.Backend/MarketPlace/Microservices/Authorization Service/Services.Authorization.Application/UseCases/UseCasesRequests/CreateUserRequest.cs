using MediatR;

namespace AuthorizationService
{
    public class CreateUserRequest : IRequest<Guid>
    {
        public UserDTO userDTO { get; set; }

        public CreateUserRequest(UserDTO userDTO)
        {
            this.userDTO = userDTO;
        }
    }
}
