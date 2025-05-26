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
        private readonly IReadUserNotificationUseCase _readUserNotificationUseCase;
        private readonly IGetUserUnreadNotificationsCountUseCase _getUserUnreadNotificationsCountUseCase;

        public UserController(IUpdateUserUseCase updateUserUseCase, 
                              IGetUserInfoUseCase getUserInfoUseCase,
                              IUpdateUserLogoUseCase updateUserLogoUseCase,
                              IReadUserNotificationUseCase readUserNotificationUseCase,
                              IGetUserUnreadNotificationsCountUseCase getUserUnreadNotificationsCountUseCase)
        {
            _updateUserUseCase = updateUserUseCase;
            _getUserInfoUseCase = getUserInfoUseCase;
            _updateUserLogoUseCase = updateUserLogoUseCase;
            _readUserNotificationUseCase = readUserNotificationUseCase;
            _getUserUnreadNotificationsCountUseCase = getUserUnreadNotificationsCountUseCase;
        }

        [Authorize(Policy = "User")]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(CancellationToken cancellationToken)
        {
            var user = await _getUserInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [Authorize(Policy = "User")]
        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDTO, CancellationToken cancellationToken)
        {
            userDTO.Id = UserId;
            await _updateUserUseCase.Execute(userDTO, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpPatch("logo")]
        public async Task<IActionResult> UpdateUserLogo([FromBody] UserLogoDTO logo, CancellationToken cancellationToken)
        {
            await _updateUserLogoUseCase.Execute(UserId, logo.base64Logo, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpPatch("notifications/{id}")]
        public async Task<IActionResult> ReadUserMessage(Guid id, CancellationToken cancellationToken)
        {
            await _readUserNotificationUseCase.Execute(UserId, id, cancellationToken);  

            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpGet("notifications/unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationsCount(CancellationToken cancellationToken)
        {
            var count = await _getUserUnreadNotificationsCountUseCase.Execute(UserId, cancellationToken);   

            return Ok(count);
        }
    }
}
