using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/manufacturer")]
    public class ManufacturerController : BaseController
    {
        private readonly IUpdateManufacturerUseCase _updateManufacturerUseCase;
        private readonly IGetManufacturerInfoUseCase _getManufacturerInfoUseCase;
        private readonly IUpdateManufacturerLogoUseCase _updateManufacturerLogoUseCase;
        private readonly IReadManufacturerNotificationUseCase _readManufacturerNotificationUseCase;
        private readonly IGetManufacturerUnreadNotificationsCountUseCase _getManufacturerUnreadNotificationsCountUseCase;

        public ManufacturerController(IUpdateManufacturerUseCase updateManufacturerUseCase, 
                                      IGetManufacturerInfoUseCase getManufacturerInfoUseCase,
                                      IUpdateManufacturerLogoUseCase updateManufacturerLogoUseCase,
                                      IReadManufacturerNotificationUseCase readManufacturerNotificationUseCase,
                                      IGetManufacturerUnreadNotificationsCountUseCase getManufacturerUnreadNotificationsCountUseCase)
        {
            _updateManufacturerUseCase = updateManufacturerUseCase;
            _getManufacturerInfoUseCase = getManufacturerInfoUseCase;
            _updateManufacturerLogoUseCase = updateManufacturerLogoUseCase;
            _readManufacturerNotificationUseCase = readManufacturerNotificationUseCase;
            _getManufacturerUnreadNotificationsCountUseCase = getManufacturerUnreadNotificationsCountUseCase;
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpGet]
        public async Task<ActionResult<Manufacturer>> GetManufacturer(CancellationToken cancellationToken)
        {
            var user = await _getManufacturerInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPatch]
        public async Task<IActionResult> UpdateManufaturer([FromBody] ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            manufacturerDTO.Id = UserId;
            await _updateManufacturerUseCase.Execute(manufacturerDTO, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPatch("logo")]
        public async Task<IActionResult> UpdateManufaturerLogo([FromBody] ManufacturerLogoDTO logo, CancellationToken cancellationToken)
        {
            await _updateManufacturerLogoUseCase.Execute(UserId, logo.base64Logo, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPatch("notifications/{id}")]
        public async Task<IActionResult> ReadUserMessage(Guid id, CancellationToken cancellationToken)
        {
            await _readManufacturerNotificationUseCase.Execute(UserId, id, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpGet("notifications/unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationsCount(CancellationToken cancellationToken)
        {
            var count = await _getManufacturerUnreadNotificationsCountUseCase.Execute(UserId, cancellationToken);

            return Ok(count);
        }
    }
}
