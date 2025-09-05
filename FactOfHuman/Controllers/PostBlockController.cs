using FactOfHuman.Dto.Post;
using FactOfHuman.Migrations;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostBlockController : ControllerBase
    {
        private readonly IPostBlockService _postBlockService;
        private readonly IWebHostEnvironment _env;
        public PostBlockController(IPostBlockService postBlockService, IWebHostEnvironment env)
        {
            _postBlockService = postBlockService;
            _env = env;
        }
        [Authorize(Roles = "Author")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PostBlock>> Post([FromForm] CreatePostBlockDto dto)
        {
            if (dto == null) return BadRequest("Dto is null");

            var topImage = string.Empty;
            var botImage = string.Empty;

            var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadFolder = Path.Combine(webrootPath, "posts");
            Directory.CreateDirectory(uploadFolder);

            // Xử lý ảnh top nếu có
            if (dto.TopImageUrl != null && dto.TopImageUrl.Length > 0)
            {
                var topFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.TopImageUrl.FileName)}";
                var topFilePath = Path.Combine(uploadFolder, topFileName);

                using (var stream = new FileStream(topFilePath, FileMode.Create))
                {
                    await dto.TopImageUrl.CopyToAsync(stream);
                }
                topImage = $"/posts/{topFileName}";
            }

            // Xử lý ảnh bottom nếu có
            if (dto.BottomImageUrl != null && dto.BottomImageUrl.Length > 0)
            {
                var botFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.BottomImageUrl.FileName)}";
                var botFilePath = Path.Combine(uploadFolder, botFileName);

                using (var stream = new FileStream(botFilePath, FileMode.Create))
                {
                    await dto.BottomImageUrl.CopyToAsync(stream);
                }
                botImage = $"/posts/{botFileName}";
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
        [Authorize (Roles = "Admin")]
        [HttpDelete("Delete-By-Id/{id}")]
        public async Task<ActionResult> DeleteById([FromRoute] Guid id)
        {
            try
            {
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
            if (dto == null) return BadRequest("Dto is null");
            var topImage = string.Empty;
            var botImage = string.Empty;
            var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadFolder = Path.Combine(webrootPath, "posts");
            Directory.CreateDirectory(uploadFolder);
            // Xử lý ảnh top nếu có
            if (dto.TopImageUrl != null && dto.TopImageUrl.Length > 0)
            {
                var topFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.TopImageUrl.FileName)}";
                var topFilePath = Path.Combine(uploadFolder, topFileName);
                using (var stream = new FileStream(topFilePath, FileMode.Create))
                {
                    await dto.TopImageUrl.CopyToAsync(stream);
                }
                topImage = $"/posts/{topFileName}";
            }
            // Xử lý ảnh bottom nếu có
            if (dto.BottomImageUrl != null && dto.BottomImageUrl.Length > 0)
            {
                var botFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.BottomImageUrl.FileName)}";
                var botFilePath = Path.Combine(uploadFolder, botFileName);
                using (var stream = new FileStream(botFilePath, FileMode.Create))
                {
                    await dto.BottomImageUrl.CopyToAsync(stream);
                }
                botImage = $"/posts/{botFileName}";
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
