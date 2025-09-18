using FactOfHuman.Dto.Reaction;
using FactOfHuman.Enum;
using FactOfHuman.Extensions;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService _reactionService;
        public ReactionController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }
        [AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll(int skip = 0, int take = 10)
        {
            var reactions = await _reactionService.GetAllAsync(skip, take);
            return Ok(reactions);
        }
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromQuery] TargetType targetType, [FromQuery] TypeReaction typeReaction, [FromBody] CreateReacionDto dto)
        {
            try
            {
                var userId = User.getUserId();
                var reaction = await _reactionService.CreateAsyn(userId.Value, dto, targetType, typeReaction);
                return Ok(reaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var userId = User.getUserId();
                if (userId == null) return Unauthorized(new { message = "You must be logged in to delete a reaction" });
                var result = await _reactionService.DeleteAsync(id, userId.Value);
                return Ok(new { message = "Reaction deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("get-by-comment/{commentId}")]
        public async Task<IActionResult> GetByComment([FromRoute] Guid commentId)
        {
            try
            {
                var count = await _reactionService.CountReactionsByTargetAsync(commentId, TargetType.Comment, TypeReaction.Like);
                var reaction = await _reactionService.GetByCommentAsync(commentId);
                return Ok(new {count,reaction});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet("get-by-post/{postId}")]
        public async Task<IActionResult> GetByPost([FromRoute] Guid postId)
        {
            try
            {
                var count = await _reactionService.CountReactionsByTargetAsync(postId, TargetType.Post, TypeReaction.Like); 
                var reaction = await _reactionService.GetByPostAsync(postId);
                return Ok(new {count,reaction});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
