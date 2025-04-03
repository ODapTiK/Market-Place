using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        private readonly IUpdateAdminUseCase _updateAdminUseCase;
        private readonly IGetAdminInfoUseCase _getAdminInfoUseCase;

        public AdminController(IUpdateAdminUseCase updateAdminUseCase, 
                               IGetAdminInfoUseCase getAdminInfoUseCase)
        {
            _updateAdminUseCase = updateAdminUseCase;
            _getAdminInfoUseCase = getAdminInfoUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<Admin>> GetAdmin(CancellationToken cancellationToken)
        {
            var admin = await _getAdminInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(admin);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAdmin([FromBody] AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            await _updateAdminUseCase.Execute(adminDTO, cancellationToken);

            return Ok();
        }
    }
}
