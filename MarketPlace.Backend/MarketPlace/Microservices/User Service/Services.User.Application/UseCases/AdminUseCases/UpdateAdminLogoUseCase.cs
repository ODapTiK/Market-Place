namespace UserService
{
    public class UpdateAdminLogoUseCase : IUpdateAdminLogoUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public UpdateAdminLogoUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task Execute(Guid adminId, string base64Image, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Admin), adminId);

            admin.Logo = base64Image;

            await _adminRepository.UpdateAsync(cancellationToken);
        }
    }
}
