using Microsoft.AspNetCore.Mvc;

namespace UserService
{
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly ICreateUserUseCase _createUserUseCase;
        private readonly IUpdateUserUseCase _updateUserUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly IAddUserOrderUseCase _addUserOrderUseCase;
        private readonly IRemoveUserOrderUseCase _removeUserOrderUseCase;
        private readonly IGetUserInfoUseCase _getUserInfoUseCase;

        public UserController(ICreateUserUseCase createUserUseCase, 
                              IUpdateUserUseCase updateUserUseCase, 
                              IDeleteUserUseCase deleteUserUseCase, 
                              IAddUserOrderUseCase addUserOrderUseCase, 
                              IRemoveUserOrderUseCase removeUserOrderUseCase, 
                              IGetUserInfoUseCase getUserInfoUseCase)
        {
            _createUserUseCase = createUserUseCase;
            _updateUserUseCase = updateUserUseCase;
            _deleteUserUseCase = deleteUserUseCase;
            _addUserOrderUseCase = addUserOrderUseCase;
            _removeUserOrderUseCase = removeUserOrderUseCase;
            _getUserInfoUseCase = getUserInfoUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUser()
        {
            var user = await _getUserInfoUseCase.Execute(UserId);

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UserDTO userDTO)
        {
            var resultId = await _createUserUseCase.Execute(userDTO);

            return Ok(resultId);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDTO)
        {
            await _updateUserUseCase.Execute(userDTO);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            await _deleteUserUseCase.Execute(UserId);

            return Ok();
        }

        [HttpPut("/Order/Add/{id}")]
        public async Task<IActionResult> AddUserOrder(Guid id)
        {
            await _addUserOrderUseCase.Execute(UserId, id);
            
            return Ok();
        }

        [HttpPut("Order/Remove/{id}")]
        public async Task<IActionResult> RemoveUserOrder(Guid id)
        {
            await _removeUserOrderUseCase.Execute(UserId, id);

            return Ok();
        }
    }
}
