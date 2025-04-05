using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    [Route("api/orders")]
    public class OrderController : BaseController
    {
        private readonly IMapper _mapper;

        public OrderController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderDTO createOrderDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateOrderCommand>(createOrderDTO);
            command.UserId = UserId;

            var resultId = await Mediator.Send(command, cancellationToken);

            return Ok(resultId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteOrderCommand()
            {
                Id = id
            };

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetOrderQuery()
            {
                Id = id
            };

            var order = await Mediator.Send(query, cancellationToken);
            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetUserOrders(CancellationToken cancellationToken)
        {
            var query = new GetUserOrdersQuery()
            {
                UserId = UserId
            };

            var userOrders = await Mediator.Send(query, cancellationToken);
            return Ok(userOrders);
        }
    }
}
