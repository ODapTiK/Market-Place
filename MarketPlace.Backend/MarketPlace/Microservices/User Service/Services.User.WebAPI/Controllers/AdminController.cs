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

        public AdminController(IUpdateAdminUseCase updateAdminUseCase, 
                               IGetAdminInfoUseCase getAdminInfoUseCase,
                               IUpdateAdminLogoUseCase updateAdminLogoUseCase)
        {
            _updateAdminUseCase = updateAdminUseCase;
            _getAdminInfoUseCase = getAdminInfoUseCase;
            _updateAdminLogoUseCase = updateAdminLogoUseCase;
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<ActionResult<Admin>> GetAdmin(CancellationToken cancellationToken)
        {
            var admin = await _getAdminInfoUseCase.Execute(UserId, cancellationToken);

            return Ok(admin);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateAdmin([FromBody] AdminDTO adminDTO, CancellationToken cancellationToken)
        {
            adminDTO.Id = UserId;
            await _updateAdminUseCase.Execute(adminDTO, cancellationToken);

            return Ok();
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("logo")]
        public async Task<IActionResult> UpdateAdminLogo([FromBody] AdminLogoDTO logo, CancellationToken cancellationToken)
        {
            await _updateAdminLogoUseCase.Execute(UserId, logo.base64Logo, cancellationToken);

            return Ok();
        }
    }
}
