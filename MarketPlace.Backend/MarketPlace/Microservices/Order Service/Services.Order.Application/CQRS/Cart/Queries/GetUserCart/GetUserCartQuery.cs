using MediatR;

namespace OrderService
{
    public class GetUserCartQuery : IRequest<Cart>
    {
        public Guid UserId { get; set; }
    }
}
