using MediatR;

namespace OrderService
{
    public class AddOrderPointCommandHandler : IRequestHandler<AddOrderPointCommand>
    {
        private readonly ICartRepository _cartRepository;

        public AddOrderPointCommandHandler(ICartRepository repository)
        {
            _cartRepository = repository;
        }

        public async Task Handle(AddOrderPointCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
            if (cart == null) 
                throw new EntityNotFoundException(nameof(Cart), request.CartId);

            await _cartRepository.AddOrderPointAsync(cart, request.ProductId, cancellationToken);
        }
    }
}
