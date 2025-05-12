using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    [Collection("Database Collection")]
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

        [Fact]
        public async Task RemoveOrderPointAsync_ShouldDoNothing_WhenProductNotInCart()
        {
            // Arrange
            var cart = _cartFaker.Generate();
            var productId = Guid.NewGuid();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _repository.RemoveOrderPointAsync(cart, productId, CancellationToken.None);

            // Assert
            var updatedCart = await _context.Carts.FindAsync(cart.Id, CancellationToken.None);
            updatedCart.Should().NotBeNull();
            updatedCart.Products.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveProductFromCartsAsync_ShouldRemoveProductFromAllCarts()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart1 = _cartFaker.Generate();
            cart1.Products.Add(productId);
            var cart2 = _cartFaker.Generate();
            cart2.Products.Add(productId);
            var cart3 = _cartFaker.Generate();
            cart3.Products.Add(Guid.NewGuid()); 

            await _context.Carts.AddRangeAsync(cart1, cart2, cart3);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            await _repository.RemoveProductFromCartsAsync(productId, CancellationToken.None);

            // Assert
            var cartsWithProduct = await _context.Carts
                .Where(c => c.Products.Contains(productId))
                .ToListAsync(CancellationToken.None);

            cartsWithProduct.Should().BeEmpty();

            var cart1Updated = await _context.Carts.FindAsync(cart1.Id, CancellationToken.None);
            cart1Updated.Products.Should().NotContain(productId);

            var cart2Updated = await _context.Carts.FindAsync(cart2.Id, CancellationToken.None);
            cart2Updated.Products.Should().NotContain(productId);

            var cart3Updated = await _context.Carts.FindAsync(cart3.Id, CancellationToken.None);
            cart3Updated.Products.Should().ContainSingle();
        }

        [Fact]
        public async Task RemoveProductFromCartsAsync_ShouldDoNothing_WhenProductNotInAnyCart()
        {
            // Arrange
            var cart = _cartFaker.Generate();
            cart.Products.Add(Guid.NewGuid());
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(CancellationToken.None);
            var productId = Guid.NewGuid();

            // Act
            await _repository.RemoveProductFromCartsAsync(productId, CancellationToken.None);

            // Assert
            var updatedCart = await _context.Carts.FindAsync(cart.Id, CancellationToken.None);
            updatedCart.Should().NotBeNull();
            updatedCart.Products.Should().ContainSingle();
        }

        [Fact]
        public async Task RemoveProductFromCartsAsync_ShouldWork_WhenNoCartsExist()
        {
            // Arrange
            _context.Carts.RemoveRange(await _context.Carts.ToListAsync());
            await _context.SaveChangesAsync(CancellationToken.None);
            var productId = Guid.NewGuid();

            // Act
            await _repository.RemoveProductFromCartsAsync(productId, CancellationToken.None);

            // Assert
            var carts = await _context.Carts.ToListAsync(CancellationToken.None);
            carts.Should().BeEmpty();
        }
    }
}
