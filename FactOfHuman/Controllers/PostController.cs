using DotNetEnv;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Post;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IWebHostEnvironment _env;
        public PostController(IPostService postService, IWebHostEnvironment env)
        {
            _postService = postService;
            _env = env;
        }
        [Authorize(Roles ="Author")]
        [HttpPost("Post")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostDto? dto)
        {
            var userId = GetUserIdFromClaims();
            string coverImage = string.Empty;
            if (dto.CoverImage != null && dto.CoverImage.Length > 0)
            {
                var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadFolder = Path.Combine(webrootPath, "posts");
                Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.CoverImage.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.CoverImage.CopyToAsync(stream);
                }

                coverImage = $"/posts/{fileName}";
                
            }
            try
            {
                var post = await _postService.CreatePostAsync(dto, coverImage, userId.Value);
                return Ok(post);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Get-All")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PostDto>>> Getall(int skip =0,int take = 30)
        {
            var post = await _postService.GetAllAsync(skip, take);
            return Ok(post);
        }
        private Guid? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }
            return userId;
        }
        [HttpGet("Get-By-Id/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostDto>> GetById([FromRoute]Guid id)
        {
            try
            {
                var post = await _postService.GetByIdAsync(id);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Get-By-Name")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PostDto>>> GetByName([FromQuery]string name, int skip = 0, int take = 30)
        {
            var post = await _postService.GetByNamePostAsync(name, skip, take);
            return Ok(post);
        }
        [HttpGet("Get-By-UserId")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PostDto>>> GetByUserId([FromRoute] Guid userId, int skip = 0, int take = 30)
        {
            var post = await _postService.GetPostsByUserIdAsync(userId, skip, take);
            return Ok(post);
        }
        [Authorize (Roles = "Author")]
        [HttpGet("Get-With-Author")]
        public async Task<ActionResult<List<PostDto>>> GetPostWithAuthor(int skip = 0, int take = 30)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized("Invalid user ID.");
            }
            var post = await _postService.GetPostWithAuthor(userId.Value, skip, take);
            return Ok(post);
        }
        [Authorize(Roles = "Author")]
        [HttpPut("Update/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PostDto>> UpdatePost([FromRoute] Guid id, [FromForm] CreatePostDto dto)
        {
            var userId = GetUserIdFromClaims();
            string coverImage = string.Empty;
            if (dto.CoverImage != null && dto.CoverImage.Length > 0)
            {
                var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadFolder = Path.Combine(webrootPath, "posts");
                Directory.CreateDirectory(uploadFolder);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.CoverImage.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.CoverImage.CopyToAsync(stream);
                }
                coverImage = $"/posts/{fileName}";
            }
            try
            {
                var post = await _postService.UpdatePostAsync(id, dto, coverImage, userId.Value);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Author,Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeletePost([FromRoute] Guid id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized("Invalid user ID.");
            }
            try
            {
                var isAdmin = User.IsInRole("Admin");
                if (isAdmin)
                {
                    await _postService.DeletePostAsyncWithAdmin(id);
                }
                else
                {
                    await _postService.DeletePostAsync(id, userId.Value);
                }
                return Ok(new { message = "Post deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
