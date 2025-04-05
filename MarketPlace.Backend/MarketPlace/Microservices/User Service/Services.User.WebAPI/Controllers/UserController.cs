using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/user")]
    public class UserController : BaseController
    {
        private readonly IUpdateUserUseCase _updateUserUseCase;
        private readonly IGetUserInfoUseCase _getUserInfoUseCase;

        public UserController(IUpdateUserUseCase updateUserUseCase, 
                              IGetUserInfoUseCase getUserInfoUseCase)
        {
            _updateUserUseCase = updateUserUseCase;
            _getUserInfoUseCase = getUserInfoUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUser(CancellationToken cancellationToken)
        {
            var user = await _getUserInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDTO, CancellationToken cancellationToken)
        {
            await _updateUserUseCase.Execute(userDTO, cancellationToken);

            return Ok();
        }
    }
}
