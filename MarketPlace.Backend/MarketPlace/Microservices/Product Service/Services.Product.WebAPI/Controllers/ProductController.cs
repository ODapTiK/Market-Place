using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ProductService
{
    [Route("api/[controller]")]
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

        [HttpGet("Manufacturer/{id}")]
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

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductDTO createProductDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateProductCommand>(createProductDTO);
            command.ManufacturerId = UserId;

            var productId = await Mediator.Send(command, cancellationToken);
            return Ok(productId);
        }

        [HttpPost("Review")]
        public async Task<ActionResult<Guid>> CreateProductReview([FromBody] CreateProductReviewDTO createProductReviewDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateProductReviewCommand>(createProductReviewDTO);
            command.UserId = UserId;

            var reviewId = await Mediator.Send(command, cancellationToken);
            return Ok(reviewId);
        }

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

        [HttpDelete("Review")]
        public async Task<IActionResult> DeleteProductReview([FromBody] DeleteProductReviewDTO deleteProductReviewDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<DeleteProductReviewCommand>(deleteProductReviewDTO);
            command.UserId = UserId;

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDTO updateProductDTO, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateProductCommand>(updateProductDTO);
            command.ManufacturerId = UserId;

            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
