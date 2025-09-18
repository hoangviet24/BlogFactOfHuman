using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.Reaction;
using FactOfHuman.Enum;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Repository.Service
{
    public class ReactionService : IReactionService
    {
        private readonly FactOfHumanDbContext _context;
        private readonly IMapper _mapper;
        public ReactionService(FactOfHumanDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> CountReactionsByTargetAsync(Guid targetId, TargetType targetType, TypeReaction typeReaction)
        {
            return await _context.Reactions
            .Where(r => r.TargetId == targetId
                 && r.TargetType == targetType
                 && r.Type == typeReaction)
            .CountAsync();
        }

        public async Task<ReactionDto> CreateAsyn(Guid userId, CreateReacionDto dto, TargetType targetType, TypeReaction typeReaction)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!user) throw new BadHttpRequestException("User not found");
            
            if( targetType == TargetType.Post)
            {
                var post = await _context.Posts.AnyAsync(p => p.Id == dto.TargetId);
                if(!post) throw new BadHttpRequestException("Post not found");
            }
            else if( targetType == TargetType.Comment)
            {
                var comment = await _context.Comments.AnyAsync(c => c.Id == dto.TargetId);
                if (!comment) throw new BadHttpRequestException("Comment not found");
            }

            var reaction = new Reaction();
            reaction = _mapper.Map<Reaction>(dto);
            reaction.UserId = userId;
            reaction.TargetId = dto.TargetId;
            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();
            return _mapper.Map<ReactionDto>(reaction);
        }
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var reaction = await _context.Reactions.Where(r => r.Id == id && r.UserId == userId).ExecuteDeleteAsync();
            if (reaction == 0) throw new BadHttpRequestException("Reaction not found or you are not the owner");
            return await Task.FromResult(true);
        }
        public async Task<List<ReactionDto>> GetAllAsync(int skip, int take)
        {
            var getAll = await _context.Reactions
                .Include(r => r.User)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            var reactionDto = _mapper.Map<List<ReactionDto>>(getAll);
            return await Task.FromResult(reactionDto);
        }

        public async Task<ReactionDto> GetByCommentAsync(Guid commentId)
        {
            var getId = await _context.Reactions.FindAsync(commentId);
            var reactionDto = _mapper.Map<ReactionDto>(getId);
            return await Task.FromResult(reactionDto);
        }

        public async Task<ReactionDto> GetByIdAsync(Guid id)
        {
            var getId = await _context.Reactions.FindAsync(id);
            var reactionDto = _mapper.Map<ReactionDto>(getId);
            return await Task.FromResult(reactionDto);
        }

        public async Task<ReactionDto> GetByPostAsync(Guid postId)
        {
            var getPost = await _context.Reactions.FindAsync(postId);
            var reactionDto = _mapper.Map<ReactionDto>(getPost);
            return await Task.FromResult(reactionDto);
        }
    }
}
