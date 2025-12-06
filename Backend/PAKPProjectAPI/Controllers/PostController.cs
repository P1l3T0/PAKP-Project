using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAKPProjectData;
using PAKPProjectServices;

namespace PAKPProjectAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController(DataContext dataContext, IUserService userService) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IUserService _userService = userService;

        [HttpGet("get-post-safe/{postId}")]
        public async Task<ActionResult> GetPostSafe(int postId)
        {
            try
            {
                // Safe: Check if user is authorized to access this post
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();
                UserPost? post = await _dataContext.UserPosts
                    .Where(p => p.ID == postId)
                    .FirstOrDefaultAsync();

                if (post is null)
                {
                    return NotFound("Post not found");
                }

                // Safe: Authorization check
                if (post.UserID != currentUser.ID && post.IsPrivate)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        Error = "You don't have permission to access this post"
                    });
                }

                return Ok(new
                {
                    Post = post.ToDto<UserPostDTO>(),
                    Method = "Safe with Authorization"
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Error = "Access denied",
                    Method = "Safe with Authorization"
                });
            }
        }

        [HttpGet("get-post-vulnerable/{postId}")]
        public async Task<ActionResult> GetPostVulnerable(int postId)
        {
            try
            {
                // VULNERABLE: No authorization check - classic IDOR
                UserPost? post = await _dataContext.UserPosts.FindAsync(postId);

                if (post is null)
                {
                    return NotFound("Post not found");
                }

                // VULNERABLE: Returns any post regardless of ownership
                return Ok(new
                {
                    Post = post.ToDto<UserPostDTO>(),
                    Method = "Vulnerable IDOR"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message,
                    Method = "Vulnerable IDOR"
                });
            }
        }

        [HttpGet("user-posts-safe")]
        public async Task<ActionResult> GetUserPostsSafe()
        {
            try
            {
                // Safe: Only returns current user's posts
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();
                List<UserPostDTO> posts = await _dataContext.UserPosts
                    .Where(p => p.UserID == currentUser.ID)
                    .Select(p => p.ToDto<UserPostDTO>())
                    .ToListAsync();

                return Ok(new
                {
                    Posts = posts,
                    Method = "Safe - Own Posts Only"
                });
            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Error = "Access denied",
                    Method = "Safe - Own Posts Only"
                });
            }
        }

        [HttpGet("user-posts-vulnerable/{userId}")]
        public async Task<ActionResult> GetUserPostsVulnerable(int userId)
        {
            try
            {
                // VULNERABLE: Can access any user's posts by changing userId
                List<UserPostDTO> posts = await _dataContext.UserPosts
                    .Where(p => p.UserID == userId)
                    .Select(p => p.ToDto<UserPostDTO>())
                    .ToListAsync();

                return Ok(new
                {
                    Posts = posts,
                    UserId = userId,
                    Method = "Vulnerable IDOR"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    Error = ex.Message, 
                    Method = "Vulnerable, IDOR"
                });
            }
        }

        [HttpPost("create-post")]
        public async Task<ActionResult> CreatePost([FromBody] CreatePostDTO createPostDto)
        {
            try
            {
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();

                UserPost post = new UserPost()
                {
                    Title = createPostDto.Title,
                    Content = createPostDto.Content,
                    IsPrivate = createPostDto.IsPrivate,
                    UserID = currentUser.ID
                };

                _dataContext.UserPosts.Add(post);
                await _dataContext.SaveChangesAsync();

                return Ok(new 
                { 
                    Message = "Post created", 
                    PostId = post.ID
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("delete-post-vulnerable/{postId}")]
        public async Task<ActionResult> DeletePostVulnerable(int postId)
        {
            try
            {
                // VULNERABLE: Can delete any post without authorization check
                UserPost? post = await _dataContext.UserPosts.FindAsync(postId);

                if (post is null)
                {
                    return NotFound("Post not found");
                }

                _dataContext.UserPosts.Remove(post);
                await _dataContext.SaveChangesAsync();

                return Ok(new 
                { 
                    Message = "Post deleted", 
                    Method = "Vulnerable IDOR"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    Error = ex.Message
                });
            }
        }
    }
}