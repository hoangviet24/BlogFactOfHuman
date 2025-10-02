using DotNetEnv;
using FactOfHuman.Data;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Post;
using FactOfHuman.Enum;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using FactOfHuman.Repository.Service;
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
        [HttpGet("Get-top-10")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PostDto>>> GetTop10()
        {
            var post = await _postService.GetTop10Async();
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
        [HttpGet("Get-By-Category/{categoryId}")]
        public async Task<ActionResult<PostDto>> GetByCategory([FromRoute] Guid categoryId, int skip = 0, int take = 30)
        {
            try
            {
                var post = await _postService.GetPostByCategory(categoryId, skip, take);
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
        public async Task<ActionResult<List<PostDto>>> GetByUserId([FromQuery] Guid userId, int skip = 0, int take = 30)
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
        [Authorize]
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
                _fileservice.DeleteFile(oldAvatarPath);
                var isAdmin = User.IsInRole("Admin");
                if (isAdmin)
                {
                    try
                    {
                        await _postService.DeletePostAsyncWithAdmin(id);
                       
                        return Ok(new { message = "Post deleted successfully." });
                    }
                    catch(Exception ex) {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        await _postService.DeletePostAsync(id,userId.Value);
                        return Ok(new { message = "Post deleted successfully." });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
