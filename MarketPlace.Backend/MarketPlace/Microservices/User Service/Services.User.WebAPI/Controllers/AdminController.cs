using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/admin")]
    public class AdminController : BaseController
    {
        private readonly IUpdateAdminUseCase _updateAdminUseCase;
        private readonly IGetAdminInfoUseCase _getAdminInfoUseCase;
        private readonly IUpdateAdminLogoUseCase _updateAdminLogoUseCase;
        private readonly IGetAdminUnreadNotificationsCountUseCase _getAdminUnreadNotificationsCountUseCase;
        private readonly IReadAdminNotificationUseCase _readAdminNotificationUseCase;

        public AdminController(IUpdateAdminUseCase updateAdminUseCase, 
                               IGetAdminInfoUseCase getAdminInfoUseCase,
                               IUpdateAdminLogoUseCase updateAdminLogoUseCase,
                               IGetAdminUnreadNotificationsCountUseCase getAdminUnreadNotificationsCountUseCase,
                               IReadAdminNotificationUseCase readAdminNotificationUseCase)
        {
            _updateAdminUseCase = updateAdminUseCase;
            _getAdminInfoUseCase = getAdminInfoUseCase;
            _updateAdminLogoUseCase = updateAdminLogoUseCase;
            _getAdminUnreadNotificationsCountUseCase = getAdminUnreadNotificationsCountUseCase;
            _readAdminNotificationUseCase = readAdminNotificationUseCase;
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<ActionResult<Admin>> GetAdmin(CancellationToken cancellationToken)
        {
            var admin = await _getAdminInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(admin);
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch]
        public async Task<IActionResult> UpdateAdmin([FromBody] AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            adminDTO.Id = UserId;
            await _updateAdminUseCase.Execute(adminDTO, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch("logo")]
        public async Task<IActionResult> UpdateAdminLogo([FromBody] AdminLogoDTO logo, CancellationToken cancellationToken)
        {
            await _updateAdminLogoUseCase.Execute(UserId, logo.base64Logo, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Admin")]
        [HttpPatch("notifications/{id}")]
        public async Task<IActionResult> ReadUserMessage(Guid id, CancellationToken cancellationToken)
        {
            await _readAdminNotificationUseCase.Execute(UserId, id, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("notifications/unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationsCount(CancellationToken cancellationToken)
        {
            var count = await _getAdminUnreadNotificationsCountUseCase.Execute(UserId, cancellationToken);

            return Ok(count);
        }
    }
}
