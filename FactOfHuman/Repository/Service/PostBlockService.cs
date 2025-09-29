using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.Post;
using FactOfHuman.Dto.PostBlock;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Repository.Service
{
    public class PostBlockService : IPostBlockService
    {
        private readonly FactOfHumanDbContext _context;
        private readonly IMapper _mapper;
        public PostBlockService(FactOfHumanDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PostBlock> CreateAsync(CreatePostBlockDto dto, string topImage, string botImage)
        {
            if (string.IsNullOrWhiteSpace(dto.TopContent)
                && string.IsNullOrWhiteSpace(dto.BottomContent)
                && dto.TopImageUrl == null
                && dto.BottomImageUrl == null)
            {
                throw new BadHttpRequestException("Phải có 1 dòng có nội dung");
            }
            var postBlock = new PostBlock {
                PostId = dto.PostId,
                TopContent = dto.TopContent ?? string.Empty,
                TopImage = topImage ?? string.Empty,
                BottomContent = dto.BottomContent ?? string.Empty,
                BottomImage = botImage ?? string.Empty,
            };
            _context.PostBlocks.Add(postBlock);
            await _context.SaveChangesAsync();
            var postdto = _mapper.Map<PostBlockDto>(postBlock);
            return postBlock;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var postBlock = _context.PostBlocks.FirstOrDefault(pb => pb.Id == id);
            if (postBlock == null)
            {
                throw new BadHttpRequestException("PostBlock not found");
            }
            _context.PostBlocks.Remove(postBlock);
            _context.SaveChanges();
            return await Task.FromResult(true);
        }

        public async Task<List<PostBlock>> GetAllAsync()
        {
            var postBlocks = await _context.PostBlocks.ToListAsync();
            return postBlocks;
        }

        public async Task<PostBlock> GetByIdAsync(Guid id)
        {
            var postBlock = _context.PostBlocks.FirstOrDefault(pb => pb.Id == id);
            if (postBlock == null)
            {
                throw new BadHttpRequestException("PostBlock not found");
            }
            return await Task.FromResult(postBlock);
        }

        public async Task<PostBlock> UpdateAsync(Guid id, UpdatePostBlockDto dto, string topImage, string botImage)
        {
            var postBlock = _context.PostBlocks.FirstOrDefault(pb => pb.Id == id);
            if (postBlock == null)
            {
                throw new BadHttpRequestException("PostBlock not found");
            }
            if (string.IsNullOrWhiteSpace(dto.TopContent)
                && string.IsNullOrWhiteSpace(dto.BottomContent)
                && dto.TopImageUrl == null
                && dto.BottomImageUrl == null)
            {
                throw new BadHttpRequestException("Phải có 1 dòng có nội dung");
            }
            postBlock.TopContent = dto.TopContent ?? postBlock.TopContent;
            postBlock.TopImage = topImage ?? postBlock.TopImage;
            postBlock.BottomContent = dto.BottomContent ?? postBlock.BottomContent;
            postBlock.BottomImage = botImage ?? postBlock.BottomImage;
            await _context.SaveChangesAsync();
            return postBlock;
        }
    }
}