using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    [Route("api/[controller]")]
    public class OrderController : BaseController
    {
        private readonly IMapper _mapper;

        public OrderController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderDTO createOrderDTO)
        {
            var command = _mapper.Map<CreateOrderCommand>(createOrderDTO);
            command.UserId = UserId;

            var resultId = await Mediator.Send(command);

            return Ok(resultId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var command = new DeleteOrderCommand()
            {
                Id = id
            };

            await Mediator.Send(command);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var query = new GetOrderQuery()
            {
                Id = id
            };

            var order = await Mediator.Send(query);
            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetUserOrders()
        {
            var query = new GetUserOrdersQuery()
            {
                UserId = UserId
            };

            var userOrders = await Mediator.Send(query);
            return Ok(userOrders);
        }
    }
}
