namespace UserService
{
    public interface IUpdateUserLogoUseCase
    {
        public Task Execute(Guid userId, string base64Image, CancellationToken cancellationToken);
    }
}
