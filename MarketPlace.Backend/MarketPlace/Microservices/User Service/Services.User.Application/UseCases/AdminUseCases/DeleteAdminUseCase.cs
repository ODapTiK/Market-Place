namespace UserService
{
    public class DeleteAdminUseCase : IDeleteAdminUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public DeleteAdminUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task Execute(Guid adminId, CancellationToken cancellationToken)
        {
            if (adminId == Guid.Empty)
                throw new FluentValidation.ValidationException("Admin Id must not be empty!");

            var admin = await _adminRepository.GetByIdAsync(adminId, cancellationToken);
            if (admin == null)
                throw new EntityNotFoundException(nameof(Admin), adminId);

            await _adminRepository.DeleteAsync(admin, cancellationToken);
        }
    }
}
