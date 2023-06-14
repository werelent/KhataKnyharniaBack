using Xunit;
using PracticeWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PracticeTests.Tests
{
    public class UserTests
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public UserTests()
        {
            var services = new ServiceCollection();

            // Configure in-memory database
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase(databaseName: "TestDatabase"));

            // Configure identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            // Create mock logger
            var loggerMock = new Mock<ILogger<UserManager<User>>>();

            // Provide mock logger
            services.AddSingleton(loggerMock.Object);

            var serviceProvider = services.BuildServiceProvider();

            _context = serviceProvider.GetRequiredService<DataContext>();
            _userManager = new UserManager<User>(new UserStore<User>(_context), null, new PasswordHasher<User>(), null, null, null, null, null, loggerMock.Object);
        }

        [Fact]
        public async Task RegisterUser_ShouldCreateUser()
        {
            // Arrange
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var password = "TestPassword123";

            // Act
            var result = await _userManager.CreateAsync(user, password);

            // Assert
            Assert.True(result.Succeeded, "User creation failed.");
            Assert.NotNull(await _userManager.FindByNameAsync("testuser"));
        }
    }
}
