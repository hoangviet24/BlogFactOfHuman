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
        [HttpPost("create/{targetType}/{typeReaction}")]
        public async Task<IActionResult> Create([FromRoute] TargetType targetType, [FromRoute] TypeReaction typeReaction, [FromBody] CreateReacionDto dto)
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
        [AllowAnonymous]
        [HttpGet("count")]
        public async Task<IActionResult> CountReactionsByTarget([FromQuery] Guid targetId, [FromQuery] TargetType targetType, [FromQuery] TypeReaction typeReaction)
        {
            var count = await _reactionService.CountReactionsByTargetAsync(targetId, targetType, typeReaction);
            Console.WriteLine($"{targetType} ({(int)targetType}), {typeReaction} ({(int)typeReaction})");
            return Ok(new { count });
        }
    }
}
