using FactOfHuman.Dto.Post;
using FactOfHuman.Enum;

namespace FactOfHuman.Repository.IService
{
    public interface IPostService
    {
        //Create Post
        Task<PostDto> CreatePostAsync(CreatePostDto dto,string coverImage, Guid userId);
        //Get Post
        Task<List<PostDto>> GetAllAsync(int skip, int take);
        Task<List<PostDto>> GetTop10Async();
        Task<List<PostDto>> GetPostByCategory(Guid categoryId, int skip, int take);
        Task<List<PostDto>> GetPostsByUserIdAsync(Guid userId, int skip, int take);
        Task<List<PostDto>> GetByNamePostAsync(string name, int skip, int take);
        Task<PostDto> GetByIdAsync(Guid id);
        Task<List<PostDto>> GetPostWithAuthor(Guid userId, int skip, int take);
        //Update Post
        Task<PostDto> UpdatePostAsync(Guid id, StatusPost status, CreatePostDto dto, string coverImage, Guid userId);
        //Delete Post
        Task<bool> DeletePostAsync(Guid id, Guid userId);
        Task<bool> DeletePostAsyncWithAdmin(Guid id);
    }
}
