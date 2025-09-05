using FactOfHuman.Dto.Category;
using FactOfHuman.Models;

namespace FactOfHuman.Repository.IService
{
    public interface ICategoryService
    {
        Task<Category> CreateAsync(CreateCategoryDto category);
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(Guid id);
        Task<Category> UpdateAsync(Guid id, CreateCategoryDto category);
        Task<bool> DeleteAsync(Guid id);
    }
}
