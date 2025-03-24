using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {

        [HttpGet("Auth")]
        public async Task<ActionResult<TokenDTO>> AuthenticateUser([FromQuery] AuthUserDTO authUserDTO)
        {
            var token = await Mediator.Send(new AuthUserRequest(authUserDTO));

            return Ok(token);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UserDTO userDTO)
        {
            var resultId = await Mediator.Send(new CreateUserRequest(userDTO));

            return Ok(resultId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await Mediator.Send(new DeleteUserRequest() { Id = id }); 

            return Ok();
        }
    }
}
