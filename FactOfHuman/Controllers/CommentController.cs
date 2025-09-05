using FactOfHuman.Dto.UserDto;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        [Authorize]
        [HttpPost("Post-Comment")]
        public async Task<ActionResult> PostComment([FromBody] CreateCommentDto dto)
        {
            if (dto == null) return BadRequest("Dto is null");
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("User ID not found in token");
            var comment = await _commentService.CreateCommentAsync(userId.Value, dto);
            return Ok(comment);
        }
        [AllowAnonymous]
        [HttpGet("Get-Comments/{postId}")]
        public async Task<ActionResult> GetCommentsByPostId(Guid postId, int skip = 0, int take = 10)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId, skip, take);
            return Ok(comments);
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
    }
}
