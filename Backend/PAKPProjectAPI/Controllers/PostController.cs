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

        [HttpGet("get-post-vulnerable/{postId}")]
        public async Task<ActionResult> GetPostVulnerable(int postId)
        {
            try
            {
                UserPost? post = await _dataContext.UserPosts.FindAsync(postId);

                if (post is null)
                {
                    return NotFound("Post not found");
                }

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

        [HttpGet("user-posts-vulnerable/{userId}")]
        public async Task<ActionResult> GetUserPostsVulnerable(int userId)
        {
            try
            {
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