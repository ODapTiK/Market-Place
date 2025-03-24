using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using FluentAssertions;

namespace UserService
{
    public class ManufacturerControllerTests
    {
        private readonly Mock<ICreateManufacturerUseCase> _createManufacturerUseCaseMock;
        private readonly Mock<IUpdateManufacturerUseCase> _updateManufacturerUseCaseMock;
        private readonly Mock<IDeleteManufacturerUseCase> _deleteManufacturerUseCaseMock;
        private readonly Mock<IGetManufacturerInfoUseCase> _getManufacturerInfoUseCaseMock;
        private readonly Mock<IAddManufacturerProductUseCase> _addManufacturerProductUseCaseMock;
        private readonly Mock<IRemoveManufacturerProductUseCase> _removeManufacturerProductUseCaseMock;
        private readonly ManufacturerController _controller;
        private readonly Faker _faker;

        public ManufacturerControllerTests()
        {
            _createManufacturerUseCaseMock = new Mock<ICreateManufacturerUseCase>();
            _updateManufacturerUseCaseMock = new Mock<IUpdateManufacturerUseCase>();
            _deleteManufacturerUseCaseMock = new Mock<IDeleteManufacturerUseCase>();
            _getManufacturerInfoUseCaseMock = new Mock<IGetManufacturerInfoUseCase>();
            _addManufacturerProductUseCaseMock = new Mock<IAddManufacturerProductUseCase>();
            _removeManufacturerProductUseCaseMock = new Mock<IRemoveManufacturerProductUseCase>();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .BuildServiceProvider();

            _controller = new ManufacturerController(
                _createManufacturerUseCaseMock.Object,
                _updateManufacturerUseCaseMock.Object,
                _deleteManufacturerUseCaseMock.Object,
                _getManufacturerInfoUseCaseMock.Object,
                _addManufacturerProductUseCaseMock.Object,
                _removeManufacturerProductUseCaseMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            _faker = new Faker();
        }

        [Fact]
        public async Task GetManufacturer_ShouldReturnOkResult_WithManufacturer()
        {
            // Arrange
            var manufacturer = new Manufacturer 
            { 
                Id = Guid.NewGuid(), 
                Organization = _faker.Company.CompanyName() 
            };
            _getManufacturerInfoUseCaseMock.Setup(u => u.Execute(_controller.UserId))
                .ReturnsAsync(manufacturer);

            // Act
            var result = await _controller.GetManufacturer();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(manufacturer);
            _getManufacturerInfoUseCaseMock.Verify(m => m.Execute(_controller.UserId), Times.Once);
        }

        [Fact]
        public async Task CreateManufacturer_ShouldReturnOkResult_WithNewManufacturerId()
        {
            // Arrange
            var manufacturerDTO = new ManufacturerDTO { Organization = _faker.Company.CompanyName() };
            var newManufacturerId = Guid.NewGuid();
            _createManufacturerUseCaseMock.Setup(u => u.Execute(manufacturerDTO))
                .ReturnsAsync(newManufacturerId);

            // Act
            var result = await _controller.CreateManufacturer(manufacturerDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(newManufacturerId);
            _createManufacturerUseCaseMock.Verify(m => m.Execute(manufacturerDTO), Times.Once);
        }

        [Fact]
        public async Task UpdateManufacturer_ShouldReturnOkResult()
        {
            // Arrange
            var manufacturerDTO = new ManufacturerDTO { Id = Guid.NewGuid(), Organization = _faker.Company.CompanyName() };

            // Act
            var result = await _controller.UpdateManufaturer(manufacturerDTO);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _updateManufacturerUseCaseMock.Verify(m => m.Execute(manufacturerDTO), Times.Once);
        }

        [Fact]
        public async Task DeleteManufacturer_ShouldReturnOkResult()
        {
            // Arrange
            var userId = _controller.UserId;

            // Act
            var result = await _controller.DeleteManufacturer();

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _deleteManufacturerUseCaseMock.Verify(m => m.Execute(userId), Times.Once);
        }

        [Fact]
        public async Task AddManufacturerProduct_ShouldReturnOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = _controller.UserId;

            // Act
            var result = await _controller.AddManufacturerProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _addManufacturerProductUseCaseMock.Verify(m => m.Execute(userId, productId), Times.Once);
        }

        [Fact]
        public async Task RemoveManufacturerProduct_ShouldReturnOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = _controller.UserId;

            // Act
            var result = await _controller.RemoveManufacturerProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _removeManufacturerProductUseCaseMock.Verify(m => m.Execute(userId, productId), Times.Once);
        }
    }
}
