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

        [HttpGet("get-post/{postId}")]
        public async Task<ActionResult> GetPost(int postId)
        {
            try
            {
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();
                UserPost? post = await _dataContext.UserPosts.FindAsync(postId);

                if (post is null)
                {
                    return NotFound("Post not found");
                }

                if (post.IsPrivate && post.UserID != currentUser.ID)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        Error = "You don't have permission to access this post",
                        Method = ""
                    });
                }

                return Ok(new
                {
                    Post = post.ToDto<UserPostDTO>(),
                    Method = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = ex.Message,
                    Method = ""
                });
            }
        }

        [HttpGet("user-posts/{userId}")]
        public async Task<ActionResult> GetUserPosts(int userId)
        {
            try
            {
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();

                var postsQuery = _dataContext.UserPosts.Where(p => p.UserID == userId);

                // If requesting someone else's posts, only show public ones
                if (userId != currentUser.ID)
                {
                    postsQuery = postsQuery.Where(p => !p.IsPrivate);
                }

                List<UserPostDTO> posts = await postsQuery
                    .Select(p => p.ToDto<UserPostDTO>())
                    .ToListAsync();

                return Ok(new
                {
                    Posts = posts,
                    UserId = userId,
                    Method = ""
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    Error = ex.Message, 
                    Method = ""
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

        [HttpDelete("delete-post/{postId}")]
        public async Task<ActionResult> DeletePost(int postId)
        {
            try
            {
                CurrentUserDTO currentUser = await _userService.GetCurrentUserAsync();
                UserPost? post = await _dataContext.UserPosts.FindAsync(postId);

                if (post is null)
                {
                    return NotFound("Post not found");
                }

                if (post.UserID != currentUser.ID)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        Error = "You don't have permission to delete this post",
                        Method = ""
                    });
                }

                _dataContext.UserPosts.Remove(post);
                await _dataContext.SaveChangesAsync();

                return Ok(new 
                { 
                    Message = "Post deleted", 
                    Method = ""
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