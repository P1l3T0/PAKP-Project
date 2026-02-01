using Microsoft.EntityFrameworkCore;
using PAKPProjectData;
using PAKPProjectServices;
using Moq;
using Xunit;

namespace PAKPProjectTests.Services;

public class AuthServiceSecurityTests
{
    private DataContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);

        // Seed test data
        context.Users.AddRange(
            new User { ID = 1, Email = "test@example.com", Username = "testuser", PasswordHash = new byte[10], PasswordSalt = new byte[10], DateCreated = DateTime.Now },
            new User { ID = 2, Email = "admin@example.com", Username = "admin", PasswordHash = new byte[10], PasswordSalt = new byte[10], DateCreated = DateTime.Now }
        );
        context.SaveChanges();

        return context;
    }

    [Theory]
    [InlineData("'; DROP TABLE Users--")]
    [InlineData("' OR '1'='1")]
    [InlineData("%'; DELETE FROM Users WHERE '1'='1")]
    [InlineData("admin'--")]
    public async Task SearchUsersWithRawSqlAsync_SqlInjectionAttempts_HandledSafely(string maliciousInput)
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockJwt = new Mock<IJwtService>();
        var mockRefresh = new Mock<IRefreshTokenService>();
        var mockCookie = new Mock<ICookieService>();
        var mockUser = new Mock<IUserService>();
        var authService = new AuthService(mockJwt.Object, mockRefresh.Object, mockCookie.Object, mockUser.Object, context);

        // Act - should not throw exception or execute malicious SQL
        var result = await authService.SearchUsersWithRawSqlAsync(maliciousInput);

        // Assert - returns empty result, no SQL injection occurred
        Assert.NotNull(result);
        Assert.Empty(result);

        // Verify database integrity - users still exist
        Assert.Equal(2, await context.Users.CountAsync());
    }

    [Fact]
    public async Task SearchUsersWithRawSqlAsync_DoesNotExposePasswordData()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mockJwt = new Mock<IJwtService>();
        var mockRefresh = new Mock<IRefreshTokenService>();
        var mockCookie = new Mock<ICookieService>();
        var mockUser = new Mock<IUserService>();
        var authService = new AuthService(mockJwt.Object, mockRefresh.Object, mockCookie.Object, mockUser.Object, context);

        // Act
        var result = await authService.SearchUsersWithRawSqlAsync("test");

        // Assert - verify no sensitive data in DTO
        Assert.NotNull(result);
        foreach (var user in result)
        {
            Assert.NotNull(user.Email);
            Assert.NotNull(user.Username);
            // PasswordHash and PasswordSalt should not be accessible in CurrentUserDTO
            Assert.DoesNotContain("PasswordHash", user.GetType().GetProperties().Select(p => p.Name));
            Assert.DoesNotContain("PasswordSalt", user.GetType().GetProperties().Select(p => p.Name));
        }
    }
}