namespace AuthorizationService
{
    public interface IAuthenticationUseCase
    {
        public Task<TokenDTO> Execute(AuthUserDTO authUserDTO, CancellationToken cancellationToken);
    }
}
