using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductService
{
    [Route("api/products")]
    public class ProductController : BaseController
    {
        private readonly IMapper _mapper;

        public ProductController(IMapper mapper)
        {
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductQuery()
            {
                Id = id
            };

            var product = await Mediator.Send(query, cancellationToken);
            return Ok(product);
        }

        [HttpGet("manufacturers/{id}")]
        public async Task<ActionResult<List<Product>>> GetManufacturerProducts(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetManufacturerProductsQuery()
            {
                ManufacturerId = id
            };

            var products = await Mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts(CancellationToken cancellationToken)
        {
            var query = new GetAllProductsQuery();

            var products = await Mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductDTO createProductDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateProductCommand>(createProductDTO);
            command.ManufacturerId = UserId;

            var productId = await Mediator.Send(command, cancellationToken);
            return Ok(productId);
        }

        [Authorize(Policy = "User")]
        [HttpPost("{productId}/reviews")]
        public async Task<ActionResult<Guid>> CreateProductReview(Guid productId, [FromBody] CreateProductReviewDTO createProductReviewDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateProductReviewCommand>(createProductReviewDTO);
            command.ProductId = productId;
            command.UserId = UserId;

            var reviewId = await Mediator.Send(command, cancellationToken);
            return Ok(reviewId);
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand()
            {
                Id = id,
                ManufacturerId = UserId
            };

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{productId}/reviews/{reviewId}")]
        public async Task<IActionResult> DeleteProductReview(Guid productId, Guid reviewId, CancellationToken cancellationToken)
        {
            var command = new DeleteProductReviewCommand()
            {
                Id = reviewId,
                ProductId = productId,
                UserId = UserId
            };  

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [Authorize(Policy = "Manufacturer")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDTO updateProductDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateProductCommand>(updateProductDTO);
            command.ManufacturerId = UserId;

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpPost("{productId}/user")]
        public async Task<IActionResult> AddProductToUserCart(Guid productId, CancellationToken cancellationToken)
        {
            var command = new AddProductToUserCartCommand()
            {
                ProductId = productId,
                UserId = UserId
            };

            await Mediator.Send(command);
            return Ok();
        }

        [Authorize(Policy = "User")]
        [HttpDelete("{productId}/user")]
        public async Task<IActionResult> RemoveProductFromUserCart(Guid productId, CancellationToken cancellationToken)
        {
            var command = new RemoveProductFromUserCartCommand()
            {
                ProductId = productId,
                UserId = UserId
            };

            await Mediator.Send(command);
            return Ok();
        }
    }
}
