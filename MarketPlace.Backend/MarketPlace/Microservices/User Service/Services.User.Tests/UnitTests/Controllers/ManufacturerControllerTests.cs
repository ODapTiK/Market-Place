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
        private readonly Mock<IUpdateManufacturerUseCase> _updateManufacturerUseCaseMock;
        private readonly Mock<IGetManufacturerInfoUseCase> _getManufacturerInfoUseCaseMock;
        private readonly ManufacturerController _controller;
        private readonly Faker _faker;

        public ManufacturerControllerTests()
        {
            _updateManufacturerUseCaseMock = new Mock<IUpdateManufacturerUseCase>();
            _getManufacturerInfoUseCaseMock = new Mock<IGetManufacturerInfoUseCase>();

            var httpContext = new DefaultHttpContext();
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            httpContext.RequestServices = new ServiceCollection()
                .BuildServiceProvider();

            _controller = new ManufacturerController(
                _updateManufacturerUseCaseMock.Object,
                _getManufacturerInfoUseCaseMock.Object)
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
            _getManufacturerInfoUseCaseMock.Setup(u => u.Execute(_controller.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manufacturer);

            // Act
            var result = await _controller.GetManufacturer(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            okResult.Value.Should().BeEquivalentTo(manufacturer);
            _getManufacturerInfoUseCaseMock.Verify(m => m.Execute(_controller.UserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateManufacturer_ShouldReturnOkResult()
        {
            // Arrange
            var manufacturerDTO = new ManufacturerDTO { Id = Guid.NewGuid(), Organization = _faker.Company.CompanyName() };

            // Act
            var result = await _controller.UpdateManufaturer(manufacturerDTO, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _updateManufacturerUseCaseMock.Verify(m => m.Execute(manufacturerDTO, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
