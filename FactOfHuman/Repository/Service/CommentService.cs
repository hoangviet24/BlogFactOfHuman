using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.CommentDto;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Repository.Service
{
    public class CommentService : ICommentService
    {
        private readonly FactOfHumanDbContext _context;
        private readonly IMapper _mapper;
        public CommentService(FactOfHumanDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CommentDto> CreateCommentAsync(Guid userId, CreateCommentDto dto)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = dto.PostId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            var commentDto = _mapper.Map<CommentDto>(comment);
            return commentDto;
        }

        public Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCommentAsyncWithAdmin(Guid commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CommentDto>> GetCommentsByPostIdAsync(Guid postId, int skip, int take)
        {
            var comments = _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Include(c => c.Post)
                .Include(c => c.Fact)
                .Skip(skip)
                .Take(take)
                .ToList();
            var commentDtos = _mapper.Map<List<CommentDto>>(comments);
            return await Task.FromResult(commentDtos);
        }

        public Task<CommentDto> UpdateCommentAsync(Guid userId, CreateCommentDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
