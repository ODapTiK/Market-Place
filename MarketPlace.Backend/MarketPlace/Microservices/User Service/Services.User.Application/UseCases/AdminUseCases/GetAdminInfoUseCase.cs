namespace UserService
{
    public class GetAdminInfoUseCase : IGetAdminInfoUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public GetAdminInfoUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<Admin> Execute(Guid adminId)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId, CancellationToken.None);
            if (admin == null)
                throw new EntityNotFoundException(nameof(Admin), adminId);

            return admin;
        }
    }
}
