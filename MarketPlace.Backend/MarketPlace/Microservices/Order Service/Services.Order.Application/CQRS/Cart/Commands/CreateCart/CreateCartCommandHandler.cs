using MediatR;

namespace OrderService
{
    public class CreateCartCommandHandler : IRequestHandler<CreateCartCommand, Guid>
    {
        private readonly ICartRepository _cartRepository;

        public CreateCartCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Guid> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
            };

            return await _cartRepository.CreateAsync(cart, cancellationToken);
        }
    }
}
