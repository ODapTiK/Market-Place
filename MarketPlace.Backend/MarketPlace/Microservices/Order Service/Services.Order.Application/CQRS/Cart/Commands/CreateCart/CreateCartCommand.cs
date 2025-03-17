using MediatR;

namespace OrderService
{
    public class CreateCartCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
    }
}
