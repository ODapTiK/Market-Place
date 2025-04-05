using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/manufacturer")]
    public class ManufacturerController : BaseController
    {
        private readonly IUpdateManufacturerUseCase _updateManufacturerUseCase;
        private readonly IGetManufacturerInfoUseCase _getManufacturerInfoUseCase;

        public ManufacturerController(IUpdateManufacturerUseCase updateManufacturerUseCase, 
                                      IGetManufacturerInfoUseCase getManufacturerInfoUseCase)
        {
            _updateManufacturerUseCase = updateManufacturerUseCase;
            _getManufacturerInfoUseCase = getManufacturerInfoUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<Manufacturer>> GetManufacturer(CancellationToken cancellationToken)
        {
            var user = await _getManufacturerInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateManufaturer([FromBody] ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            await _updateManufacturerUseCase.Execute(manufacturerDTO, cancellationToken);

            return Ok();
        }
    }
}
