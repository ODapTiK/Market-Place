using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCart()
        {
            var command = new CreateCartCommand()
            {
                UserId = UserId
            };

            var resultId = await Mediator.Send(command);
            return Ok(resultId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(Guid id)
        {
            var command = new DeleteCartCommand()
            {
                Id = id
            };

            await Mediator.Send(command);
            return Ok();
        }

        [HttpPut("AddPoint/{CartId}/{ProductId}")]
        public async Task<IActionResult> AddOrderPoint(Guid CartId, Guid ProductId)
        {
            var command = new AddOrderPointCommand()
            {
                ProductId = ProductId,
                CartId = CartId
            };

            await Mediator.Send(command);
            return Ok();
        }

        [HttpPut("RemovePoint/{CartId}/{ProductId}")]
        public async Task<IActionResult> RemoveOrderPoint(Guid CartId, Guid ProductId)
        {
            var command = new RemoveOrderPointCommand()
            {
                ProductId = ProductId,
                CartId = CartId
            };

            await Mediator.Send(command);
            return Ok();
        }
    }
}
