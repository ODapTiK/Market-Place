namespace AuthorizationService
{
    public interface IAuthDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
