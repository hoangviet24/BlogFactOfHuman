using FactOfHuman.Dto.Tag;
using FactOfHuman.Models;

namespace FactOfHuman.Repository.IService
{
    public interface ITagService
    {
        Task<Tag> PostAsync(TagDto dto);
        Task<List<Tag>> GetAllAsync();
        Task<Tag> GetByIdAsync(Guid id);
        Task<Tag> UpdateAsync(Guid id, TagDto dto);
        Task<bool> DeleteAsync(Guid id);

    }
}
