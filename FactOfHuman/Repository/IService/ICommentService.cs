using FactOfHuman.Dto.CommentDto;

namespace FactOfHuman.Repository.IService
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentDto dto);
        Task<List<CommentDto>> GetAllCommentAsync( int skip, int take);
        Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId, int skip, int take);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
        Task<bool> DeleteCommentAsyncWithAdmin(Guid commentId);
        Task<bool> DeleteCommentOnPostAuthorAsync(Guid commentId, Guid postAuthorId);
        Task<bool> DeleteAllCommentOnPostAuthorAsync(Guid postId, Guid postAuthorId);
        Task<CommentDto> UpdateCommentAsync(Guid userId, CommentDto dto);
    }
}
