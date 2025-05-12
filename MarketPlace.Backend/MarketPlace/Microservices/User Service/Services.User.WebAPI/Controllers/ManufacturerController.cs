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

        public ManufacturerController(IUpdateManufacturerUseCase updateManufacturerUseCase, 
                                      IGetManufacturerInfoUseCase getManufacturerInfoUseCase,
                                      IUpdateManufacturerLogoUseCase updateManufacturerLogoUseCase)
        {
            _updateManufacturerUseCase = updateManufacturerUseCase;
            _getManufacturerInfoUseCase = getManufacturerInfoUseCase;
            _updateManufacturerLogoUseCase = updateManufacturerLogoUseCase;
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpGet]
        public async Task<ActionResult<Manufacturer>> GetManufacturer(CancellationToken cancellationToken)
        {
            var user = await _getManufacturerInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPut]
        public async Task<IActionResult> UpdateManufaturer([FromBody] ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            manufacturerDTO.Id = UserId;
            await _updateManufacturerUseCase.Execute(manufacturerDTO, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPut("logo")]
        public async Task<IActionResult> UpdateManufaturerLogo([FromBody] ManufacturerLogoDTO logo, CancellationToken cancellationToken)
        {
            await _updateManufacturerLogoUseCase.Execute(UserId, logo.base64Logo, cancellationToken);

            return Ok();
        }
    }
}
