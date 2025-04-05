using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService
{
    [Route("api/user")]
    public class UserController : BaseController
    {

        [HttpGet("auth")]
        public async Task<ActionResult<TokenDTO>> AuthenticateUser([FromQuery] AuthUserDTO authUserDTO, CancellationToken cancellationToken)
        {
            var token = await Mediator.Send(new AuthUserRequest(authUserDTO));

            return Ok(token);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UserDTO userDTO, CancellationToken cancellationToken)
        {
            var resultId = await Mediator.Send(new CreateUserRequest(userDTO));

            return Ok(resultId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteUserRequest() { Id = id }); 

            return Ok();
        }
    }
}
