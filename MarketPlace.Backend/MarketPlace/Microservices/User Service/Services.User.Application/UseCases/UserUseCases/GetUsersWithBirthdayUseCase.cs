
namespace UserService
{
    public class GetUsersWithBirthdayUseCase : IGetUsersWithBirthdayUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUsersWithBirthdayUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> Execute(CancellationToken cancellationToken)
        {
            return await _userRepository.GetManyAsync(x => x.BirthDate.Day == DateTime.Now.ToUniversalTime().Day &&
                                                           x.BirthDate.Month == DateTime.Now.ToUniversalTime().Month, 
                                                           cancellationToken);   
        }
    }
}
