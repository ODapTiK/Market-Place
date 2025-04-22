using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        public async Task<ActionResult<Guid>> CreateUser([FromBody] object userDTO, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(userDTO);
            var role = JsonDocument.Parse(json).RootElement.GetProperty("Role").GetString();

            UserDTO createUserDTO;
            switch (role)
            {
                case nameof(Role.User):
                    createUserDTO = JsonSerializer.Deserialize<CreateUserDTO>(json);
                    break;
                case nameof(Role.Admin):
                    createUserDTO = JsonSerializer.Deserialize<CreateAdminDTO>(json);
                    break;
                case nameof(Role.Manufacturer):
                    createUserDTO = JsonSerializer.Deserialize<CreateManufacturerDTO>(json);
                    break;
                default:
                    return BadRequest("Invalid role");
            }

            var resultId = await Mediator.Send(new CreateUserRequest(createUserDTO));

            return Ok(resultId);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteUserRequest() { Id = UserId }); 

            return Ok();
        }
    }
}
