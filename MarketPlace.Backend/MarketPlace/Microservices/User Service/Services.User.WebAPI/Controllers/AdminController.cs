using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        private readonly ICreateAdminUseCase _createAdminUseCase;
        private readonly IUpdateAdminUseCase _updateAdminUseCase;
        private readonly IDeleteAdminUseCase _deleteAdminUseCase;
        private readonly IGetAdminInfoUseCase _getAdminInfoUseCase;

        public AdminController(ICreateAdminUseCase createAdminUseCase, 
                               IUpdateAdminUseCase updateAdminUseCase, 
                               IDeleteAdminUseCase deleteAdminUseCase, 
                               IGetAdminInfoUseCase getAdminInfoUseCase)
        {
            _createAdminUseCase = createAdminUseCase;
            _updateAdminUseCase = updateAdminUseCase;
            _deleteAdminUseCase = deleteAdminUseCase;
            _getAdminInfoUseCase = getAdminInfoUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<Admin>> GetAdmin(CancellationToken cancellationToken)
        {
            var admin = await _getAdminInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(admin);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateAdmin([FromBody] AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            var resultId = await _createAdminUseCase.Execute(adminDTO, cancellationToken);

            return Ok(resultId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAdmin([FromBody] AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            await _updateAdminUseCase.Execute(adminDTO, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAdmin(CancellationToken cancellationToken)
        {
            await _deleteAdminUseCase.Execute(UserId, cancellationToken);

            return Ok();
        }
    }
}
