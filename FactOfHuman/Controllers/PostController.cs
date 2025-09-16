using DotNetEnv;
using FactOfHuman.Data;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Post;
using FactOfHuman.Enum;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IWebHostEnvironment _env;
        private readonly FactOfHumanDbContext _context;
        private readonly IFileSerivce _fileservice;
        public PostController(IPostService postService, IWebHostEnvironment env, FactOfHumanDbContext context, IFileSerivce fileservice)
        {
            _postService = postService;
            _env = env;
            _context = context;
            _fileservice = fileservice;
        }
        [Authorize(Roles ="Author,Admin")]
        [HttpPost("Post")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PostDto>> CreatePost([FromForm] CreatePostDto? dto)
        {
            var userId = User.getUserId();
            string coverImage = string.Empty;
            if (dto.CoverImage != null && dto.CoverImage.Length > 0)
            {
                coverImage = _fileservice.SaveFile(dto.CoverImage, "posts");
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
        public async Task<ActionResult<List<PostDto>>> Getall(int skip = 0,int take = 30)
        {
            var post = await _postService.GetAllAsync(skip, take);
            return Ok(post);
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
            var userId = User.getUserId();
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
        public async Task<ActionResult<PostDto>> UpdatePost([FromRoute] Guid id, [FromForm] CreatePostDto dto, [FromQuery] StatusPost status)
        {
            var userId = User.getUserId();
            var postId = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            string coverImage = string.Empty;
            
            if (dto.CoverImage != null && dto.CoverImage.Length > 0)
            {
                _fileservice.DeleteFile(postId.CoverImage);
                coverImage = _fileservice.SaveFile(dto.CoverImage, "posts");
            }
            try
            {
                var post = await _postService.UpdatePostAsync(id,status, dto, coverImage, userId.Value);
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
            var postId = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            string oldAvatarPath = string.Empty;
            if (!string.IsNullOrEmpty(postId?.CoverImage))
            {
                oldAvatarPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), postId.CoverImage.TrimStart('/'));
            }
            var userId = User.getUserId();
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
                _fileservice.DeleteFile(oldAvatarPath);
                return Ok(new { message = "Post deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
