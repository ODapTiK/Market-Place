using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace ProductService
{
    public class ProductControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductController _controller;
        private readonly Faker _faker;

        public ProductControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();

            _faker = new Faker();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(_mediatorMock.Object)
                .BuildServiceProvider();

            _controller = new ProductController(_mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task GetProduct_ShouldReturnOk_WithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expectedProduct = new Product
            {
                Id = productId,
                Name = _faker.Commerce.ProductName(),
                Price = _faker.Finance.Amount()
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _controller.GetProduct(productId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedProduct);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetProductQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetManufacturerProducts_ShouldReturnOk_WithProducts()
        {
            // Arrange
            var manufacturerId = Guid.NewGuid();
            var expectedProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.0M },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 20.0M }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetManufacturerProductsQuery>(), default))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetManufacturerProducts(manufacturerId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedProducts);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetManufacturerProductsQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WithProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.0M },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 20.0M }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), default))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetAllProducts(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedProducts);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProductsQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnOk_WithProductId()
        {
            // Arrange
            var createProductDTO = new CreateProductDTO
            {
                Name = _faker.Commerce.ProductName(),
                Price = _faker.Finance.Amount()
            };
            var command = new CreateProductCommand
            {
                Name = createProductDTO.Name,
                Price = createProductDTO.Price,
                ManufacturerId = Guid.NewGuid() 
            };
            var expectedProductId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<CreateProductCommand>(createProductDTO))
                .Returns(command);
            _mediatorMock.Setup(m => m.Send(command, default))
                .ReturnsAsync(expectedProductId);

            // Act
            var result = await _controller.CreateProduct(createProductDTO, CancellationToken.None);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedProductId);
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnOk()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteProduct(productId, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProductCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnOk()
        {
            // Arrange
            var updateProductDTO = new UpdateProductDTO
            {
                Id = Guid.NewGuid(),
                Name = _faker.Commerce.ProductName(),
                Price = _faker.Finance.Amount()
            };
            var command = new UpdateProductCommand
            {
                Id = updateProductDTO.Id,
                Name = updateProductDTO.Name,
                Price = updateProductDTO.Price,
                ManufacturerId = Guid.NewGuid() 
            };

            _mapperMock.Setup(m => m.Map<UpdateProductCommand>(updateProductDTO))
                .Returns(command);

            // Act
            var result = await _controller.UpdateProduct(updateProductDTO, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task CreateProductReview_ShouldReturnOk_WithReviewId()
        {
            // Arrange
            var createProductReviewDTO = new CreateProductReviewDTO
            {
                Raiting = _faker.Random.Int(1, 5),
                Description = _faker.Lorem.Sentence()
            };
            var productId = Guid.NewGuid();

            var command = new CreateProductReviewCommand
            {
                ProductId = productId,
                Raiting = createProductReviewDTO.Raiting,
                Description = createProductReviewDTO.Description,
                UserId = Guid.NewGuid() 
            };
            var expectedReviewId = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<CreateProductReviewCommand>(createProductReviewDTO))
                .Returns(command);
            _mediatorMock.Setup(m => m.Send(command, default))
                .ReturnsAsync(expectedReviewId);

            // Act
            var result = await _controller.CreateProductReview(productId, createProductReviewDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedReviewId);
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task DeleteProductReview_ShouldReturnOk()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var command = new DeleteProductReviewCommand
            {
                Id = reviewId,
                ProductId = productId,
                UserId = _controller.UserId,
            };

            // Act
            var result = await _controller.DeleteProductReview(productId, reviewId, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProductReviewCommand>(), default), Times.Once);
        }
    }
}
