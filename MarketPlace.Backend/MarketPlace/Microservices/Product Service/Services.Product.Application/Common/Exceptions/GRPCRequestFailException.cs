namespace ProductService
{
    public class GRPCRequestFailException : Exception
    {
        public GRPCRequestFailException(string message) : base(message) { }
    }
}
