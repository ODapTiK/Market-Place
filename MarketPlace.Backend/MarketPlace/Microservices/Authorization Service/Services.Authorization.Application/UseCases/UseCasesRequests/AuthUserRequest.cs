using MediatR;

namespace AuthorizationService
{
    public class AuthUserRequest : IRequest<TokenDTO>
    {
        public AuthUserDTO authUserDTO;

        public AuthUserRequest(AuthUserDTO authUserDTO)
        {
            this.authUserDTO = authUserDTO;
        }
    }
}
