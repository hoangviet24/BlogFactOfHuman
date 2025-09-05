using FactOfHuman.Dto.Post;
using FactOfHuman.Models;

namespace FactOfHuman.Repository.IService
{
    public interface IPostBlockService
    {
        Task<PostBlock> CreateAsync(CreatePostBlockDto dto,string topImage, string botImage);
        Task<List<PostBlock>> GetAllAsync();
        Task<bool> DeleteAsync(Guid id);
        Task<PostBlock> GetByIdAsync(Guid id);
        Task<PostBlock> UpdateAsync(Guid id, UpdatePostBlockDto dto, string topImage, string botImage);
    }
}
