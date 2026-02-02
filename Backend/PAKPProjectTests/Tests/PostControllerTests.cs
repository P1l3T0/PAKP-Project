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
    public async Task GetPost_OwnPost_ReturnsOk()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" });

        dataContext.UserPosts.Add(new UserPost
        {
            ID = 100,
            Title = "My Post",
            Content = "Content",
            IsPrivate = false,
            UserID = 1
        });
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.GetPost(100);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetPost_OthersPrivatePost_ReturnsForbidden()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" });

        dataContext.UserPosts.Add(new UserPost
        {
            ID = 200,
            Title = "Private Post",
            Content = "Private Content",
            IsPrivate = true,
            UserID = 2
        });
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.GetPost(200);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetPost_NonExistent_ReturnsNotFound()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" });

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.GetPost(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeletePost_OwnPost_DeletesSuccessfully()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" });

        dataContext.UserPosts.Add(new UserPost
        {
            ID = 100,
            Title = "My Post",
            Content = "Content",
            IsPrivate = false,
            UserID = 1
        });
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.DeletePost(100);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Null(await dataContext.UserPosts.FindAsync(100));
    }

    [Fact]
    public async Task DeletePost_OthersPost_ReturnsNotFound()
    {
        // Arrange
        var dataContext = CreateInMemoryDataContext();
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(new CurrentUserDTO { ID = 1, Username = "user1", Email = "user1@test.com" });

        dataContext.UserPosts.Add(new UserPost
        {
            ID = 200,
            Title = "Other User's Post",
            Content = "Content",
            IsPrivate = false,
            UserID = 2
        });
        await dataContext.SaveChangesAsync();

        var controller = new PostController(dataContext, mockUserService.Object);

        // Act
        var result = await controller.DeletePost(200);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(await dataContext.UserPosts.FindAsync(200)); // Post still exists
    }
}