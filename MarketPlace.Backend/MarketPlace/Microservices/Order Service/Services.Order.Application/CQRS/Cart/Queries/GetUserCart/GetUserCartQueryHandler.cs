using MediatR;

namespace OrderService
{
    public class GetUserCartQueryHandler : IRequestHandler<GetUserCartQuery, Cart>
    {
        private readonly ICartRepository _cartRepository;

        public GetUserCartQueryHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> Handle(GetUserCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetUserCartAsync(request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException(nameof(Cart), request.UserId);

            return cart;
        }
    }
}
