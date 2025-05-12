using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/user")]
    public class UserController : BaseController
    {
        private readonly IUpdateUserUseCase _updateUserUseCase;
        private readonly IGetUserInfoUseCase _getUserInfoUseCase;
        private readonly IUpdateUserLogoUseCase _updateUserLogoUseCase;

        public UserController(IUpdateUserUseCase updateUserUseCase, 
                              IGetUserInfoUseCase getUserInfoUseCase,
                              IUpdateUserLogoUseCase updateUserLogoUseCase)
        {
            _updateUserUseCase = updateUserUseCase;
            _getUserInfoUseCase = getUserInfoUseCase;
            _updateUserLogoUseCase = updateUserLogoUseCase;
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(CancellationToken cancellationToken)
        {
            var user = await _getUserInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [Authorize(Policy = "User")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDTO, CancellationToken cancellationToken)
        {
            userDTO.Id = UserId;
            await _updateUserUseCase.Execute(userDTO, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpPut("logo")]
        public async Task<IActionResult> UpdateUserLogo([FromBody] UserLogoDTO logo, CancellationToken cancellationToken)
        {
            await _updateUserLogoUseCase.Execute(UserId, logo.base64Logo, cancellationToken);

            return Ok();
        }
    }
}
