namespace UserService
{
    public class GetAllAdminsUseCase : IGetAllAdminsUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public GetAllAdminsUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<Admin>> Execute(CancellationToken cancellationToken)
        {
            var admins = await _adminRepository.GetAllAsync(cancellationToken);

            return admins;
        }
    }
}
