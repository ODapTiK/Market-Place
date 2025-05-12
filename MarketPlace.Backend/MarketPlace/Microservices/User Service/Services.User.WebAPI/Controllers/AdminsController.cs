using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/admins")]
    public class AdminsController : BaseController
    {

        private readonly IGetAllAdminsUseCase _getAllAdminsUseCase;

        public AdminsController(IGetAllAdminsUseCase getAllAdminsUseCase)
        {
            _getAllAdminsUseCase = getAllAdminsUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<List<Admin>>> GetAllAdmins(CancellationToken cancellationToken)
        {
            var admins = await _getAllAdminsUseCase.Execute(cancellationToken); 

            return Ok(admins);
        }
    }
}
