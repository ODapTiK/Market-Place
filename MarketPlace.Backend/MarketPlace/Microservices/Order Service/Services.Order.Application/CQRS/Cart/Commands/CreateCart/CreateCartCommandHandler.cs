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
            var userCart = await _cartRepository.GetUserCartAsync(request.UserId, CancellationToken.None);
            if(userCart != null)
                throw new EntityAlreadyExistsException(nameof(Cart), request.UserId);

            var cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
            };

            return await _cartRepository.CreateAsync(cart, cancellationToken);
        }
    }
}
