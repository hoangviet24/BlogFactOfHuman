using FactOfHuman.Dto.Tag;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Tag>> Post(TagDto dto)
        {
            try
            {
                var tag = await _tagService.PostAsync(dto);
                return Ok(tag);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("Get-All")]
        public async Task<ActionResult<List<Tag>>> GetAll()
        {
            var tags = await _tagService.GetAllAsync();
            return Ok(tags);
        }
        [AllowAnonymous]
        [HttpGet("Get-By-Id/{id}")]
        public async Task<ActionResult<Tag>> GetById([FromRoute]Guid id)
        {
            try
            {
                var tag = await _tagService.GetByIdAsync(id);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize (Roles = "Admin")]
        [HttpPut("Update-By-Id/{id}")]
        public async Task<ActionResult<Tag>> PutTag([FromRoute]Guid id, TagDto dto)
        {
            try
            {
                var tag = await _tagService.UpdateAsync(id, dto);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize (Roles = "Admin")] 
        [HttpDelete("Delete-By-Id/{id}")]
        public async Task<ActionResult> DeleteTag([FromRoute]Guid id)
        {
            try
            {
                var result = await _tagService.DeleteAsync(id);
                return Ok(new { Deleted = result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
