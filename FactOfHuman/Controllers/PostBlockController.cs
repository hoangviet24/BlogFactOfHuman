using FactOfHuman.Data;
using FactOfHuman.Dto.Post;
using FactOfHuman.Dto.PostBlock;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostBlockController : ControllerBase
    {
        private readonly IPostBlockService _postBlockService;
        private readonly IWebHostEnvironment _env;
        private readonly IFileSerivce _fileSerivce;
        private readonly FactOfHumanDbContext _context;
        public PostBlockController(
            IPostBlockService postBlockService, 
            IWebHostEnvironment env, 
            IFileSerivce fileSerivce,
            FactOfHumanDbContext context)
        {
            _postBlockService = postBlockService;
            _env = env;
            _fileSerivce = fileSerivce;
            _context = context;
        }
        [Authorize(Roles = "Author")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PostBlock>> Post([FromForm] CreatePostBlockDto dto)
        {
            if (dto == null) return BadRequest("Dto is null");

            var topImage = string.Empty;
            var botImage = string.Empty;
            // Xử lý ảnh top nếu có
            if (dto.TopImageUrl != null && dto.TopImageUrl.Length > 0)
            {
                topImage = _fileSerivce.SaveFile(dto.TopImageUrl, "postsblocks");
            }

            // Xử lý ảnh bottom nếu có
            if (dto.BottomImageUrl != null && dto.BottomImageUrl.Length > 0)
            {
                botImage = _fileSerivce.SaveFile(dto.BottomImageUrl, "postsblocks");
            }

            try
            {
                var post = await _postBlockService.CreateAsync(dto, topImage, botImage);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("Get-All")]
        public async Task<ActionResult<List<PostBlock>>> GetAll()
        {
            var postBlocks = await _postBlockService.GetAllAsync();
            return Ok(postBlocks);
        }
        [AllowAnonymous]
        [HttpGet("Get-By-Id/{id}")]
        public async Task<ActionResult<PostBlock>> GetById([FromRoute] Guid id)
        {
            try
            {
                var postBlock = await _postBlockService.GetByIdAsync(id);
                return Ok(postBlock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize (Roles = "Author")]
        [HttpDelete("Delete-By-Id/{id}")]
        public async Task<ActionResult> DeleteById([FromRoute] Guid id)
        {
            var postblockId = await _context.PostBlocks.FirstOrDefaultAsync(p => p.Id == id);
            string oldTopPath = string.Empty;
            if (!string.IsNullOrEmpty(postblockId.TopImage))
            {
                oldTopPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), postblockId.TopImage.TrimStart('/'));
            }
            string oldBottomPath = string.Empty;
            if (!string.IsNullOrEmpty(postblockId.BottomImage)) {
                oldBottomPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), postblockId.BottomImage.TrimStart('/'));    
            }
            try
            {
                _fileSerivce.DeleteFile(oldTopPath);
                _fileSerivce.DeleteFile(oldBottomPath);
                var result = await _postBlockService.DeleteAsync(id);
                if (result)
                {
                    return Ok(new { message = "PostBlock deleted successfully" });
                }
                else
                {
                    return BadRequest("Failed to delete PostBlock");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize (Roles = "Author")]
        [HttpPut("Update-By-Id/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PostBlock>> Put([FromRoute] Guid id, [FromForm] UpdatePostBlockDto dto)
        {
            var postblockId = await _context.PostBlocks.FirstOrDefaultAsync(p => p.Id == id);
            if (dto == null) return BadRequest("Dto is null");
            var topImage = string.Empty;
            var botImage = string.Empty;
            // Xử lý ảnh top nếu có
            if (dto.TopImageUrl != null && dto.TopImageUrl.Length > 0)
            {
                _fileSerivce.DeleteFile(postblockId.TopImage);
                topImage = _fileSerivce.SaveFile(dto.TopImageUrl,"postsblocks");
            }
            // Xử lý ảnh bottom nếu có
            if (dto.BottomImageUrl != null && dto.BottomImageUrl.Length > 0)
            {
                _fileSerivce.DeleteFile(postblockId.BottomImage);
                botImage = _fileSerivce.SaveFile(dto.BottomImageUrl,"postsblocks");
            }
            try
            {
                var postBlock = await _postBlockService.UpdateAsync(id, dto, topImage, botImage);
                return Ok(postBlock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
