using Bogus;
using FluentAssertions;

namespace OrderService
{
    public class CartRepositoryTests : IClassFixture<TestOrderDatabaseFixture>
    {
        private readonly CartRepository _repository;
        private readonly IOrderDbContext _context;
        private readonly Faker<Cart> _cartFaker;

        public CartRepositoryTests(TestOrderDatabaseFixture fixture)
        {
            _context = fixture._context;
            _repository = new CartRepository(_context);

            _cartFaker = new Faker<Cart>()
                .RuleFor(c => c.Id, f => Guid.NewGuid())
                .RuleFor(c => c.UserId, f => Guid.NewGuid())
                .RuleFor(c => c.Products, f => new List<Guid>());
        }

        [Fact]
        public async Task AddOrderPointAsync_ShouldAddProductToCart()
        {
            // Arrange
            var cart = _cartFaker.Generate();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(CancellationToken.None);
            var productId = Guid.NewGuid();

            // Act
            await _repository.AddOrderPointAsync(cart, productId, CancellationToken.None);

            // Assert
            var updatedCart = await _context.Carts.FindAsync(cart.Id, CancellationToken.None);
            updatedCart.Should().NotBeNull();
            updatedCart.Products.Should().Contain(productId);
        }

        [Fact]
        public async Task GetUserCartAsync_ShouldReturnCart_WhenCartExists()
        {
            // Arrange
            var cart = _cartFaker.Generate();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _repository.GetUserCartAsync(cart.UserId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(cart.UserId);
        }

        [Fact]
        public async Task GetUserCartAsync_ShouldReturnNull_WhenCartDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid(); 

            // Act
            var result = await _repository.GetUserCartAsync(userId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RemoveOrderPointAsync_ShouldRemoveProductFromCart()
        {
            // Arrange
            var cart = _cartFaker.Generate();
            var productId = Guid.NewGuid();
            cart.Products.Add(productId);
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _repository.RemoveOrderPointAsync(cart, productId, CancellationToken.None);

            // Assert
            var updatedCart = await _repository.GetUserCartAsync(cart.UserId, CancellationToken.None);
            updatedCart.Should().NotBeNull();
            updatedCart.Products.Should().NotContain(productId);
        }
    }
}
