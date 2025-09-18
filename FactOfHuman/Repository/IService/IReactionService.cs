using FactOfHuman.Dto.Reaction;
using FactOfHuman.Enum;

namespace FactOfHuman.Repository.IService
{
    public interface IReactionService
    {
        Task<ReactionDto> CreateAsyn(Guid userId,CreateReacionDto dto, TargetType targetType, TypeReaction typeReaction);
        Task<List<ReactionDto>> GetAllAsync(int skip, int take);
        Task<ReactionDto> GetByIdAsync(Guid id);
        Task<ReactionDto> GetByPostAsync(Guid postId);
        Task<ReactionDto> GetByCommentAsync(Guid commentId);
        Task<int> CountReactionsByTargetAsync(Guid targetId, TargetType targetType, TypeReaction typeReaction);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
