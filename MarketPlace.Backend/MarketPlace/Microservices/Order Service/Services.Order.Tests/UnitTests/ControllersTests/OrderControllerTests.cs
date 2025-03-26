using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace OrderService
{
    public class OrderControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderController _controller;
        private readonly Faker _faker;

        public OrderControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();

            _faker = new Faker();

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(_mediatorMock.Object)
                .BuildServiceProvider();

            _controller = new OrderController(_mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnOk_WithOrderId()
        {
            // Arrange
            var createOrderDTO = new CreateOrderDTO
            {
                Points = (List<OrderPoint>)_faker.Make(_faker.Random.Int(1, 10), () => new OrderPoint()
                {
                    Id = _faker.Random.Guid(),
                    OrderId = _faker.Random.Guid(),
                    ProductId = _faker.Random.Guid(),
                    NumberOfUnits = _faker.Random.Int(1, 5)
                }),
                TotalPrice = _faker.Random.Decimal()
            };
            var command = new CreateOrderCommand 
            { 
                UserId = _faker.Random.Guid(),
                Points = (List<OrderPoint>)_faker.Make(_faker.Random.Int(1, 10), () => new OrderPoint()
                {
                    Id = _faker.Random.Guid(),
                    OrderId = _faker.Random.Guid(),
                    ProductId = _faker.Random.Guid(),
                    NumberOfUnits = _faker.Random.Int(1, 5)
                }),
                TotalPrice = _faker.Random.Decimal()
            };
            var expectedOrderId = _faker.Random.Guid();


            _mediatorMock.Setup(m => m.Send(command, default))
                .ReturnsAsync(expectedOrderId);
            _mapperMock.Setup(m => m.Map<CreateOrderCommand>(It.IsAny<CreateOrderDTO>()))
                .Returns(command);

            // Act
            var result = await _controller.CreateOrder(createOrderDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedOrderId);
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task DeleteOrder_ShouldReturnOk()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            // Act
            var result = await _controller.DeleteOrder(orderId, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteOrderCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOk_WithOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedOrder = new Order
            {
                Id = orderId,
                UserId = _faker.Random.Guid(),
                OrderDateTime = _faker.Date.Past(),
                OrderPoints = (List<OrderPoint>)_faker.Make(_faker.Random.Int(1, 10), () => new OrderPoint()
                {
                    Id = _faker.Random.Guid(),
                    OrderId = _faker.Random.Guid(),
                    ProductId = _faker.Random.Guid(),
                    NumberOfUnits = _faker.Random.Int(1, 5)
                }),
                TotalPrice = _faker.Random.Decimal()
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetOrderQuery>(), default))
                .ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.GetOrder(orderId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedOrder);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetOrderQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task GetUserOrders_ShouldReturnOk_WithUserOrders()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var expectedOrders = (List<Order>)_faker.Make(_faker.Random.Int(1, 10), () => new Order
            {
                Id = _faker.Random.Guid(),
                UserId = userId,
                OrderDateTime = _faker.Date.Past(),
                OrderPoints = (List<OrderPoint>)_faker.Make(_faker.Random.Int(1, 10), () => new OrderPoint()
                {
                    Id = _faker.Random.Guid(),
                    OrderId = _faker.Random.Guid(),
                    ProductId = _faker.Random.Guid(),
                    NumberOfUnits = _faker.Random.Int(1, 5)
                }),
                TotalPrice = _faker.Random.Decimal()
            });

            _mediatorMock.Setup(m => m.Send(It.IsAny < GetUserOrdersQuery > (), default))
                .ReturnsAsync(expectedOrders);

            // Act
            var result = await _controller.GetUserOrders(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(expectedOrders);
            _mediatorMock.Verify(m => m.Send(It.IsAny < GetUserOrdersQuery > (), default), Times.Once);
        }
    }
}
