using AutoMapper;
using MediatR;

namespace OrderService
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderPoints = request.Points,
                TotalPrice = request.TotalPrice,
                OrderDateTime = DateTime.Now.ToUniversalTime()
            };

            return _orderRepository.CreateAsync(order, cancellationToken);
        }
    }
}
