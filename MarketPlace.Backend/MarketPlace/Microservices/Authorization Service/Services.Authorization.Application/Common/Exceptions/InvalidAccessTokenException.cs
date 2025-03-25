namespace AuthorizationService
{
    public class InvalidAccessTokenException : Exception
    {
        public InvalidAccessTokenException(string name, object key) : base($"Invalid access token format!. An error occured while trying to get \"{name}\"({key})") { }
    }
}
