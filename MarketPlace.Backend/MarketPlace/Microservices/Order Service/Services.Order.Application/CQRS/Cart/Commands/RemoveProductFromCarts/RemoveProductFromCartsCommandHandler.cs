using MediatR;

namespace OrderService
{
    public class RemoveProductFromCartsCommandHandler : IRequestHandler<RemoveProductFromCartsCommand>
    {
        private readonly ICartRepository _cartRepository;

        public RemoveProductFromCartsCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task Handle(RemoveProductFromCartsCommand request, CancellationToken cancellationToken)
        {
            await _cartRepository.RemoveProductFromCartsAsync(request.ProductId, cancellationToken);
        }
    }
}
