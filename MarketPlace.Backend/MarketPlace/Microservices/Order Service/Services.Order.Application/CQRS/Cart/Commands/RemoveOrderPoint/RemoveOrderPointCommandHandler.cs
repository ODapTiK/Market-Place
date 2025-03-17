using MediatR;

namespace OrderService
{
    public class RemoveOrderPointCommandHandler : IRequestHandler<RemoveOrderPointCommand>
    {
        private readonly ICartRepository _cartRepository;

        public RemoveOrderPointCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task Handle(RemoveOrderPointCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Cart), request.CartId);

            await _cartRepository.RemoveOrderPointAsync(cart, request.ProductId, cancellationToken);
        }
    }
}
