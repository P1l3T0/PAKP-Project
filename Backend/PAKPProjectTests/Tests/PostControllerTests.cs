using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAKPProjectAPI;
using PAKPProjectData;
using PAKPProjectServices;
using Moq;

public class PostControllerTests
{
    private DataContext CreateInMemoryDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new DataContext(options);
    }

    [Fact]
    public async Task GetPost_UserCanAccessOwnPost_ReturnsOk()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();

        var currentUser = new CurrentUserDTO { ID = 1, Username = "testuser", Email = "test@test.com" };
        mockUserService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);

        var post = new UserPost
        {
            ID = 100,
            Title = "Test Post",
            Content = "Test Content",
            IsPrivate = false,
            UserID = 1
        };
        dataContext.UserPosts.Add(post);
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.GetPost(100);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetPost_UserCannotAccessOthersPost_ReturnsNotFound()
    {
        // Arrange - Regression test for authorization fix
        // Previously, users could access any post by ID without ownership check
        // This test ensures the fix prevents unauthorized access
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();

        // Current user is user ID 1
        var currentUser = new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" };
        mockUserService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);

        // Post belongs to user ID 2 (different user)
        var otherUsersPost = new UserPost
        {
            ID = 200,
            Title = "Other User's Post",
            Content = "Private Content",
            IsPrivate = false,
            UserID = 2
        };
        dataContext.UserPosts.Add(otherUsersPost);
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act - User 1 tries to access User 2's post
        var result = await controller.GetPost(200);

        // Assert - Should return NotFound with access denied message
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Post not found or access denied", notFoundResult.Value);
    }

    [Fact]
    public async Task GetPost_NonExistentPost_ReturnsNotFound()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();

        var currentUser = new CurrentUserDTO { ID = 1, Username = "testuser", Email = "test@test.com" };
        mockUserService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act - Try to get a post that doesn't exist
        var result = await controller.GetPost(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Post not found or access denied", notFoundResult.Value);
    }

    [Fact]
    public async Task DeletePost_UserCanDeleteOwnPost_ReturnsOk()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();

        var currentUser = new CurrentUserDTO { ID = 1, Username = "testuser", Email = "test@test.com" };
        mockUserService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);

        var post = new UserPost
        {
            ID = 100,
            Title = "Test Post",
            Content = "Test Content",
            IsPrivate = false,
            UserID = 1
        };
        dataContext.UserPosts.Add(post);
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.DeletePost(100);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        // Verify post was actually deleted
        var deletedPost = await dataContext.UserPosts.FindAsync(100);
        Assert.Null(deletedPost);
    }

    [Fact]
    public async Task DeletePost_UserCannotDeleteOthersPost_ReturnsNotFound()
    {
        // Arrange - Regression test for authorization fix
        // Previously, users could delete any post by ID without ownership check
        // This test ensures the fix prevents unauthorized deletion
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();

        // Current user is user ID 1
        var currentUser = new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" };
        mockUserService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);

        // Post belongs to user ID 2 (different user)
        var otherUsersPost = new UserPost
        {
            ID = 200,
            Title = "Other User's Post",
            Content = "Important Content",
            IsPrivate = false,
            UserID = 2
        };
        dataContext.UserPosts.Add(otherUsersPost);
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act - User 1 tries to delete User 2's post
        var result = await controller.DeletePost(200);

        // Assert - Should return NotFound with access denied message
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Post not found or access denied", notFoundResult.Value);

        // Verify post was NOT deleted
        var post = await dataContext.UserPosts.FindAsync(200);
        Assert.NotNull(post);
    }

    [Fact]
    public async Task DeletePost_NonExistentPost_ReturnsNotFound()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();

        var currentUser = new CurrentUserDTO { ID = 1, Username = "testuser", Email = "test@test.com" };
        mockUserService.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act - Try to delete a post that doesn't exist
        var result = await controller.DeletePost(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Post not found or access denied", notFoundResult.Value);
    }
}