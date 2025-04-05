namespace UserService
{
    public class GRPCRequestFailException : Exception
    {
        public GRPCRequestFailException(string message) : base(message) { }
    }
}
