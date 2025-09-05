using FactOfHuman.Dto.CommentDto;
using FactOfHuman.Dto.UserDto;

namespace FactOfHuman.Repository.IService
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentDto dto);
        Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId, int skip, int take);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
        Task<bool> DeleteCommentAsyncWithAdmin(Guid commentId);
        Task<CommentDto> UpdateCommentAsync(Guid userId, CreateCommentDto dto);
    }
}
