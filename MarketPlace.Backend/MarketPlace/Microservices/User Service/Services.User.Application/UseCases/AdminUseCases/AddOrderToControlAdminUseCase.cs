namespace UserService
{
    public class AddOrderToControlAdminUseCase : IAddOrderToControlAdminUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public AddOrderToControlAdminUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task Execute(Guid adminId, Guid orderId, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Admin), adminId);

            admin.AdminControlOrdersId.Add(orderId);

            await _adminRepository.UpdateAsync(cancellationToken);
        }
    }
}
