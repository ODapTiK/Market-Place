using MediatR;

namespace OrderService
{
    public class GetCartByIdQuery : IRequest<Cart>
    {
        public Guid Id { get; set; }
    }
}
