namespace AuthorizationService
{
    public class RefreshTokenBadRequestException : Exception
    {
        public RefreshTokenBadRequestException() : base("Operation failed, refresh token is invalid!") { }
    }
}
