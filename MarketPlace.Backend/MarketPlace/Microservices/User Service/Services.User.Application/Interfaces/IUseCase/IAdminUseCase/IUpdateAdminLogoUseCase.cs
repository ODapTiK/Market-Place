namespace UserService
{
    public interface IUpdateAdminLogoUseCase
    {
        public Task Execute(Guid adminId, string base64Image, CancellationToken cancellationToken);
    }
}
