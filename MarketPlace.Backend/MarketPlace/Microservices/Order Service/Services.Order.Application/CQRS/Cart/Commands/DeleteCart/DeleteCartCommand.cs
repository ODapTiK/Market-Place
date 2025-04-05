using MediatR;

namespace OrderService
{
    public class DeleteCartCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
