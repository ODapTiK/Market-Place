using AutoMapper;
using MediatR;

namespace OrderService
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                OrderPoints = request.Points,
                TotalPrice = request.TotalPrice,
                OrderDateTime = DateTime.Now.ToUniversalTime(),
            };

            return _orderRepository.CreateAsync(order, cancellationToken);
        }
    }
}
