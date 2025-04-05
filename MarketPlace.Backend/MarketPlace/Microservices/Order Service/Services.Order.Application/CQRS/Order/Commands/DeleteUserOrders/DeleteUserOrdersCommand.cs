using MediatR;

namespace OrderService
{
    public class DeleteUserOrdersCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
