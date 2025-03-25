using MediatR;

namespace OrderService
{
    public class GetCartByIdQueryHandler : IRequestHandler<GetCartByIdQuery, Cart>
    {
        private readonly ICartRepository _cartRepository;

        public GetCartByIdQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Cart), request.Id);

            return cart;
        }
    }
}
