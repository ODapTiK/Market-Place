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
        public async Task<ActionResult<Admin>> GetAdmin()
        {
            var admin = await _getAdminInfoUseCase.Execute(UserId);

            return Ok(admin);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateAdmin([FromBody] AdminDTO adminDTO)
        {
            var resultId = await _createAdminUseCase.Execute(adminDTO);

            return Ok(resultId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAdmin([FromBody] AdminDTO adminDTO)
        {
            await _updateAdminUseCase.Execute(adminDTO);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAdmin()
        {
            await _deleteAdminUseCase.Execute(UserId);

            return Ok();
        }
    }
}
