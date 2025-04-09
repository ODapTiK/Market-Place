namespace UserService
{
    public class BirthdayGreetingsGenerator
    {
        private readonly IGetUsersWithBirthdayUseCase _getUsersWithBirthdayUseCase;

        public BirthdayGreetingsGenerator(IGetUsersWithBirthdayUseCase getUsersWithBirthdayUseCase)
        {
            _getUsersWithBirthdayUseCase = getUsersWithBirthdayUseCase;
        }

        public async Task GenerateBirthdayGreetings(CancellationToken cancellationToken)
        {
            var usersToGreet = await _getUsersWithBirthdayUseCase.Execute(cancellationToken);

            //TO DO
            //Send birthday greetings to user
        }
    }
}
