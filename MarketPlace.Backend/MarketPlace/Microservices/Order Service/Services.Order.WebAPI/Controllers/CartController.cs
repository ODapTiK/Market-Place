using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    [Route("api/carts")]
    public class CartController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<Cart>> GetUserCart()
        {
            var query = new GetUserCartQuery
            {
                UserId = UserId
            };

            var userCart = await Mediator.Send(query);
            return Ok(userCart);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(Guid id)
        {
            var query = new GetCartByIdQuery
            {
                Id = id
            };

            var cart = await Mediator.Send(query);
            return Ok(cart);    
        }

        [HttpPost("{CartId}/products/{ProductId}")]
        public async Task<IActionResult> AddOrderPoint(Guid CartId, Guid ProductId, CancellationToken cancellationToken)
        {
            var command = new AddOrderPointCommand()
            {
                ProductId = ProductId,
                CartId = CartId
            };

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpDelete("{CartId}/products/{ProductId}")]
        public async Task<IActionResult> RemoveOrderPoint(Guid CartId, Guid ProductId, CancellationToken cancellationToken)
        {
            var command = new RemoveOrderPointCommand()
            {
                ProductId = ProductId,
                CartId = CartId
            };

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
