using FactOfHuman.Dto.Category;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize (Roles ="Admin")]
        [HttpPost("Post-Category")]
        public async Task<ActionResult<Category>> PostCategory([FromBody]CreateCategoryDto dto)
        {
            try
            {
                var cate = await _categoryService.CreateAsync(dto);
                return Ok(cate);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("Get-All")]
        public async Task<ActionResult<List<Category>>> GetAll()
        {
            var cate = await _categoryService.GetAllAsync();
            return Ok(cate);
        }
        [AllowAnonymous]
        [HttpGet("Get-By-Id/{id}")]
        public async Task<ActionResult<Category>> GetById([FromRoute]Guid id)
        {
            try
            {
                var cate = await _categoryService.GetByIdAsync(id);
                return Ok(cate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize (Roles ="Admin")]
        [HttpPut("Update-By-Id/{id}")]
        public async Task<ActionResult<Category>> PutCategory([FromRoute]Guid id, CreateCategoryDto dto)
        {
            try
            {
                var cate = await _categoryService.UpdateAsync(id, dto);
                return Ok(cate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("Delete-By-Id/{id}")]
        public async Task<ActionResult> DeleteCategory([FromRoute]Guid id)
        {
            try
            {
                var cate = await _categoryService.DeleteAsync(id);
                return Ok(cate);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
