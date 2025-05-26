namespace UserService
{
    public class RemoveControlAdminOrderUseCase : IRemoveControlAdminOrderUseCase
    {
        private readonly IAdminRepository _adminRepository;

        public RemoveControlAdminOrderUseCase(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task Execute(Guid adminId, Guid orderId, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByIdAsync(adminId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Admin), adminId);

            admin.AdminControlOrdersId.Remove(orderId);

            await _adminRepository.UpdateAsync(cancellationToken);
        }
    }
}
