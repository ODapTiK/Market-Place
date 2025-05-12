namespace AuthorizationService
{
    public record TokenDTO(string accessToken, string refreshToken, string role);
}
