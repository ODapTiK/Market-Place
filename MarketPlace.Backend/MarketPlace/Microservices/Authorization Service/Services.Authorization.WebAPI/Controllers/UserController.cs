using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationUseCase _authenticationUseCase;
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;

        public UserController(IAuthenticationUseCase authenticationUseCase,
                              ICreateUserUseCase createUserUseCase,
                              IDeleteUserUseCase deleteUserUseCase)
        {
            _authenticationUseCase = authenticationUseCase;
            _createUserUseCase = createUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }

        [HttpGet("Auth")]
        public async Task<ActionResult<TokenDTO>> AuthenticateUser([FromQuery] AuthUserDTO authUserDTO)
        {
            var token = await _authenticationUseCase.Execute(authUserDTO);

            return Ok(token);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UserDTO userDTO)
        {
            var resultId = await _createUserUseCase.Execute(userDTO);

            return Ok(resultId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _deleteUserUseCase.Execute(id);

            return Ok();
        }
    }
}
