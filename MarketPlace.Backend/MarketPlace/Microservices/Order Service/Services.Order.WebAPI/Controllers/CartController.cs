using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    [Route("api/carts")]
    public class CartController : BaseController
    {
        [Authorize(Policy = "User")]
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
    }
}
