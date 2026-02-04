using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PAKPProjectAPI;
using PAKPProjectData;
using PAKPProjectServices;
using Xunit;

namespace PAKPProjectTests
{
    public class PostControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly DataContext _dataContext;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dataContext = new DataContext(options);

            // Setup mock user service
            _mockUserService = new Mock<IUserService>();

            // Create controller instance
            _controller = new PostController(_dataContext, _mockUserService.Object);
        }

        [Fact]
        public async Task GetUserPosts_ReturnsOkWithPosts_WhenUserHasPosts()
        {
            // Arrange
            var currentUser = new CurrentUserDTO
            {
                ID = 1,
                Username = "testuser",
                Email = "test@example.com"
            };

            var posts = new List<UserPost>
            {
                new UserPost { ID = 1, Title = "Post 1", Content = "Content 1", UserID = 1, IsPrivate = false },
                new UserPost { ID = 2, Title = "Post 2", Content = "Content 2", UserID = 1, IsPrivate = true }
            };

            _dataContext.UserPosts.AddRange(posts);
            await _dataContext.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetCurrentUserAsync())
                .ReturnsAsync(currentUser);

            // Act
            var result = await _controller.GetUserPosts(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var response = okResult.Value;
            var postsProperty = response.GetType().GetProperty("Posts")?.GetValue(response) as List<UserPostDTO>;
            var userIdProperty = response.GetType().GetProperty("UserId")?.GetValue(response);

            Assert.NotNull(postsProperty);
            Assert.Equal(2, postsProperty.Count);
            Assert.Equal(1, userIdProperty);
            Assert.Equal("Post 1", postsProperty[0].Title);
            Assert.Equal("Post 2", postsProperty[1].Title);
        }

        [Fact]
        public async Task GetUserPosts_ReturnsOkWithEmptyList_WhenUserHasNoPosts()
        {
            // Arrange
            var currentUser = new CurrentUserDTO
            {
                ID = 1,
                Username = "testuser",
                Email = "test@example.com"
            };

            _mockUserService.Setup(x => x.GetCurrentUserAsync())
                .ReturnsAsync(currentUser);

            // Act
            var result = await _controller.GetUserPosts(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var response = okResult.Value;
            var postsProperty = response.GetType().GetProperty("Posts")?.GetValue(response) as List<UserPostDTO>;

            Assert.NotNull(postsProperty);
            Assert.Empty(postsProperty);
        }

        [Fact]
        public async Task GetUserPosts_ReturnsOnlyCurrentUserPosts_WhenMultipleUsersHavePosts()
        {
            // Arrange
            var currentUser = new CurrentUserDTO
            {
                ID = 1,
                Username = "testuser",
                Email = "test@example.com"
            };

            var posts = new List<UserPost>
            {
                new UserPost { ID = 1, Title = "User 1 Post", Content = "Content 1", UserID = 1, IsPrivate = false },
                new UserPost { ID = 2, Title = "User 2 Post", Content = "Content 2", UserID = 2, IsPrivate = false },
                new UserPost { ID = 3, Title = "User 1 Another Post", Content = "Content 3", UserID = 1, IsPrivate = true }
            };

            _dataContext.UserPosts.AddRange(posts);
            await _dataContext.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetCurrentUserAsync())
                .ReturnsAsync(currentUser);

            // Act
            var result = await _controller.GetUserPosts(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            var postsProperty = response.GetType().GetProperty("Posts")?.GetValue(response) as List<UserPostDTO>;

            Assert.NotNull(postsProperty);
            Assert.Equal(2, postsProperty.Count);
            Assert.All(postsProperty, p => Assert.Equal(1, p.UserID));
        }

        [Fact]
        public async Task GetUserPosts_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            _mockUserService.Setup(x => x.GetCurrentUserAsync())
                .ThrowsAsync(new Exception("User service error"));

            // Act
            var result = await _controller.GetUserPosts(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);

            var response = badRequestResult.Value;
            var errorProperty = response.GetType().GetProperty("Error")?.GetValue(response);

            Assert.Equal("User service error", errorProperty);
        }

        [Fact]
        public async Task GetUserPosts_IgnoresRouteParameter_UsesCurrentUser()
        {
            // Arrange
            // Note: This test documents a potential bug - the userId route parameter is ignored
            var currentUser = new CurrentUserDTO
            {
                ID = 5,
                Username = "actualuser",
                Email = "actual@example.com"
            };

            var posts = new List<UserPost>
            {
                new UserPost { ID = 1, Title = "User 5 Post", Content = "Content", UserID = 5, IsPrivate = false },
                new UserPost { ID = 2, Title = "User 3 Post", Content = "Content", UserID = 3, IsPrivate = false }
            };

            _dataContext.UserPosts.AddRange(posts);
            await _dataContext.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetCurrentUserAsync())
                .ReturnsAsync(currentUser);

            // Act - passing userId=3 but should get posts for userId=5
            var result = await _controller.GetUserPosts(3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            var postsProperty = response.GetType().GetProperty("Posts")?.GetValue(response) as List<UserPostDTO>;
            var userIdProperty = response.GetType().GetProperty("UserId")?.GetValue(response);

            Assert.NotNull(postsProperty);
            Assert.Single(postsProperty);
            Assert.Equal(5, userIdProperty); // Returns current user's ID, not route parameter
            Assert.Equal(5, postsProperty[0].UserID);
        }
    }
}
