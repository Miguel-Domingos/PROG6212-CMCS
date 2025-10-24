using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PROG6212_CMCS.Server.Controllers;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;
using Xunit;

namespace PROG6212_CMCS.Tests.Controllers
{
    public class AuthControllerTests
    {
        private ApplicationDbContext GetFreshContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private IConfiguration GetTestConfig()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "YourSuperSecretKeyForTesting12345!"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            using var context = GetFreshContext();
            var config = GetTestConfig();
            var controller = new AuthController(context, config);

            var user = new User
            {
                UserId = 1,
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                RoleId = 1
            };
            context.Users.Add(user);
            context.Roles.Add(new Role { RoleId = 1, RoleName = "Admin" });
            context.SaveChanges();

            var loginRequest = new LoginRequest { Email = "test@test.com", Password = "password123" };

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            using var context = GetFreshContext();
            var config = GetTestConfig();
            var controller = new AuthController(context, config);

            var loginRequest = new LoginRequest { Email = "wrong@test.com", Password = "wrongpass" };

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Register_ValidData_ReturnsSuccess()
        {
            // Arrange
            using var context = GetFreshContext();
            var config = GetTestConfig();
            var controller = new AuthController(context, config);

            var registerRequest = new RegisterRequest
            {
                Name = "New User",
                Email = "new@test.com",
                Password = "password123",
                RoleId = 1
            };

            // Act
            var result = controller.Register(registerRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            using var context = GetFreshContext();
            var config = GetTestConfig();
            var controller = new AuthController(context, config);

            var user = new User
            {
                UserId = 1,
                Email = "duplicate@test.com",
                PasswordHash = "hash",
                RoleId = 1
            };
            context.Users.Add(user);
            context.SaveChanges();

            var registerRequest = new RegisterRequest
            {
                Name = "Another User",
                Email = "duplicate@test.com",
                Password = "password123",
                RoleId = 1
            };

            // Act
            var result = controller.Register(registerRequest);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}