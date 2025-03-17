namespace AuthorizationService
{
    public interface IJwtProvider
    {
        public Task<TokenDTO> GenerateToken(User user, bool populateExp, CancellationToken cancellationToken);
        public Task<TokenDTO> RefreshToken(TokenDTO tokenDto);
    }
}
