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
        public async Task<ActionResult<Manufacturer>> GetManufacturer()
        {
            var user = await _getManufacturerInfoUseCase.Execute(UserId);

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateManufacturer([FromBody] ManufacturerDTO manufacturerDTO)
        {
            var resultId = await _createManufacturerUseCase.Execute(manufacturerDTO);

            return Ok(resultId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateManufaturer([FromBody] ManufacturerDTO manufacturerDTO)
        {
            await _updateManufacturerUseCase.Execute(manufacturerDTO);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteManufacturer()
        {
            await _deleteManufacturerUseCase.Execute(UserId);

            return Ok();
        }

        [HttpPut("/Product/Add/{id}")]
        public async Task<IActionResult> AddManufacturerProduct(Guid id)
        {
            await _addManufacturerProductUseCase.Execute(UserId, id);

            return Ok();
        }

        [HttpPut("Product/Remove/{id}")]
        public async Task<IActionResult> RemoveManufacturerProduct(Guid id)
        {
            await _removeManufacturerProductUseCase.Execute(UserId, id);

            return Ok();
        }
    }
}
