using FactOfHuman.Data;
using FactOfHuman.Dto.Category;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Repository.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly FactOfHumanDbContext _context;
        public CategoryService(FactOfHumanDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateAsync(CreateCategoryDto category)
        {
            var cateExits = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
            if (cateExits != null) {
                return cateExits;
            }
            if (category.Name == string.Empty)
            {
                throw new BadHttpRequestException("Name is Required");
            }
            var cate = new Category
            {
                Name = category.Name,
                Slug = SlugHelper.GenerateSlug(category.Name),
                Description = category.Description,
            };
            _context.Categories.Add(cate);
            await _context.SaveChangesAsync();
            return await Task.FromResult(cate);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var cate = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cate == null)
            {
                throw new BadHttpRequestException("Category not found");
            }
            _context.Categories.Remove(cate);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public Task<List<Category>> GetAllAsync()
        {
            var categories = _context.Categories.ToList();
            return Task.FromResult(categories);
        }

        public Task<Category> GetByIdAsync(Guid id)
        {
            var cate = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cate == null)
            {
                throw new BadHttpRequestException("Category not found");
            }
            return Task.FromResult(cate);
        }

        public Task<Category> UpdateAsync(Guid id, CreateCategoryDto category)
        {
            var cate = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cate == null)
            {
                throw new BadHttpRequestException("Category not found");
            }
            cate.Name = category.Name;
            cate.Slug = SlugHelper.GenerateSlug(category.Name);
            cate.Description = category.Description;
            _context.SaveChanges();
            return Task.FromResult(cate);
        }
    }
}
