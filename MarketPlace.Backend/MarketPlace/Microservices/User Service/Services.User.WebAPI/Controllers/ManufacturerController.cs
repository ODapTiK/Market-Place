using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/[controller]")]
    public class ManufacturerController : BaseController
    {
        private readonly ICreateManufacturerUseCase _createManufacturerUseCase;
        private readonly IUpdateManufacturerUseCase _updateManufacturerUseCase;
        private readonly IDeleteManufacturerUseCase _deleteManufacturerUseCase;
        private readonly IGetManufacturerInfoUseCase _getManufacturerInfoUseCase;
        private readonly IAddManufacturerProductUseCase _addManufacturerProductUseCase;
        private readonly IRemoveManufacturerProductUseCase _removeManufacturerProductUseCase;

        public ManufacturerController(ICreateManufacturerUseCase createManufacturerUseCase, 
                                      IUpdateManufacturerUseCase updateManufacturerUseCase, 
                                      IDeleteManufacturerUseCase deleteManufacturerUseCase, 
                                      IGetManufacturerInfoUseCase getManufacturerInfoUseCase, 
                                      IAddManufacturerProductUseCase addManufacturerProductUseCase, 
                                      IRemoveManufacturerProductUseCase removeManufacturerProductUseCase)
        {
            _createManufacturerUseCase = createManufacturerUseCase;
            _updateManufacturerUseCase = updateManufacturerUseCase;
            _deleteManufacturerUseCase = deleteManufacturerUseCase;
            _getManufacturerInfoUseCase = getManufacturerInfoUseCase;
            _addManufacturerProductUseCase = addManufacturerProductUseCase;
            _removeManufacturerProductUseCase = removeManufacturerProductUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<Manufacturer>> GetManufacturer(CancellationToken cancellationToken)
        {
            var user = await _getManufacturerInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateManufacturer([FromBody] ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            var resultId = await _createManufacturerUseCase.Execute(manufacturerDTO, cancellationToken);

            return Ok(resultId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateManufaturer([FromBody] ManufacturerDTO manufacturerDTO, CancellationToken cancellationToken)
        {
            await _updateManufacturerUseCase.Execute(manufacturerDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteManufacturer(CancellationToken cancellationToken)
        {
            await _deleteManufacturerUseCase.Execute(UserId, cancellationToken);

            return Ok();
        }

        [HttpPut("/Product/Add/{id}")]
        public async Task<IActionResult> AddManufacturerProduct(Guid id, CancellationToken cancellationToken)
        {
            await _addManufacturerProductUseCase.Execute(UserId, id, cancellationToken);

            return Ok();
        }

        [HttpPut("Product/Remove/{id}")]
        public async Task<IActionResult> RemoveManufacturerProduct(Guid id, CancellationToken cancellationToken)
        {
            await _removeManufacturerProductUseCase.Execute(UserId, id, cancellationToken);

            return Ok();
        }
    }
}
