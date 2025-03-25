namespace UserService
{
    public class GetAdminInfoUseCase : IGetAdminInfoUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public GetAdminInfoUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<Admin> Execute(Guid adminId, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId, cancellationToken);
            if (admin == null)
                throw new EntityNotFoundException(nameof(Admin), adminId);

            return admin;
        }
    }
}
