using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationService
{
    public class TestUserDatabaseFixture : IDisposable
    {
        public AuthDbContext _context { get; private set; }
        public UserManager<User> _userManager { get; private set; }
        public RoleManager<IdentityRole<Guid>> _roleManager { get; private set; }

        private readonly IServiceScope _scope;
        private bool _isDisposed = false;

        public TestUserDatabaseFixture()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(ToString());

            var services = new ServiceCollection();

            services.AddDbContext<AuthDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            services.AddLogging();



            var serviceProvider = services.BuildServiceProvider();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "Admin" }).Wait();
            _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            _scope = serviceProvider.CreateScope();
            _context = serviceProvider.GetRequiredService<AuthDbContext>();
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _context.Database.EnsureDeleted();
                _context.Dispose();
                _scope?.Dispose();
            }
        }
    }
}
