using FactOfHuman.Data;
using FactOfHuman.Dto.Tag;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Repository.Service
{
    public class TagService : ITagService
    {
        private readonly FactOfHumanDbContext _context;
        public TagService(FactOfHumanDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Id == id);
            if (tag == null)
            {
                throw new BadHttpRequestException("Tag not found");
            }
            _context.Tags.Remove(tag);
            _context.SaveChanges();
            return await Task.FromResult(true);
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            var tags = await _context.Tags.ToListAsync();
            return tags;
        }

        public async Task<Tag> GetByIdAsync(Guid id)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
            {
                throw new BadHttpRequestException("Tag not found");
            }
            return tag;
        }

        public async Task<Tag> PostAsync(TagDto dto)
        {
            var tagExits = await _context.Tags.FirstOrDefaultAsync(t => t.Name == dto.Name);
            if(dto.Name == string.Empty)
            {
                throw new BadHttpRequestException("Name is Required");
            }
            if (tagExits != null) {
                return tagExits;
            }
            var tag = new Tag() { 
                Slug = SlugHelper.GenerateSlug(dto.Name),
                Name = dto.Name,
            };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> UpdateAsync(Guid id, TagDto dto)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id); 
            if (tag == null)
            {
                throw new BadHttpRequestException("Tag not found");
            }
            tag.Name = dto.Name;
            tag.Slug = SlugHelper.GenerateSlug(dto.Name);
            await _context.SaveChangesAsync();
            return tag;
        }
    }
}
