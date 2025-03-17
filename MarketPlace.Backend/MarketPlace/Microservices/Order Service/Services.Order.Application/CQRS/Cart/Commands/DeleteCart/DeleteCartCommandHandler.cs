using MediatR;

namespace OrderService
{
    public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand>
    {
        private readonly ICartRepository _cartRepository;

        public DeleteCartCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task Handle(DeleteCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
            if (cart == null) 
                throw new EntityNotFoundException(nameof(Cart), request.Id);

            await _cartRepository.DeleteAsync(cart, cancellationToken);
        }
    }
}
