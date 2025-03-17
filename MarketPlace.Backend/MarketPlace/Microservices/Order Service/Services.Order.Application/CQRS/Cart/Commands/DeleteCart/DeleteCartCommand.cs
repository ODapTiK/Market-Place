using MediatR;

namespace OrderService
{
    public class DeleteCartCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
