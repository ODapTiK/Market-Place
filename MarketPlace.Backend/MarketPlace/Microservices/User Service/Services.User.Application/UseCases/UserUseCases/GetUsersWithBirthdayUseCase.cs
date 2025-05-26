
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
            var today = DateTime.UtcNow.Date;

            return await _userRepository.GetManyAsync(x => x.BirthDate.ToLocalTime().Day == today.Day &&
                                                           x.BirthDate.ToLocalTime().Month == today.Month, 
                                                           cancellationToken);   
        }
    }
}
