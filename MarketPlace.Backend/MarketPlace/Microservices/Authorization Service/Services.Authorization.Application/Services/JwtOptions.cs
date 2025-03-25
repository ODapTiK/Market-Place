namespace AuthorizationService
{
    public class JwtOptions : IJwtOptions
    {
        public string Key { get; set; } = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY is not set in environment variables");
        public int ExpiredMinutes { get; set; } = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED_TIME") ?? throw new InvalidOperationException("JWT_EXPIRED_TIME is not set in environment variables"));
    }
}
