using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.Post;
using FactOfHuman.Enum;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;

namespace FactOfHuman.Repository.Service
{
    public class PostService : IPostService
    {
        private readonly FactOfHumanDbContext _context;
        private readonly IMapper _mapper;
        public PostService(FactOfHumanDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto,string coverImage, Guid userId)
        {
            var tag = await _context.Tags
                .Where(tag => dto.Tags.Contains(tag.Id))
                .ToListAsync();
            var post = new Post
            {
                Slug = SlugHelper.GenerateSlug(dto.Title),
                Title = dto.Title,
                Summary = dto.Summary,
                CategoryId = dto.CategoryId,
                Tags = tag,
                CoverImage = coverImage,
                ReadingMinutes = dto.ReadingMinutes,
            };
            post.AuthorId = userId;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            var postDto = _mapper.Map<PostDto>(post);
            return postDto;
        }

        public async Task<bool> DeletePostAsync(Guid id, Guid userId)
        {
            var deletePost = _context.Posts.FirstOrDefault(p => p.Id == id && p.AuthorId == userId);
            if (deletePost == null)
            {
                throw new Exception("Post not found");
            }
            _context.Posts.Remove(deletePost);
            _context.SaveChanges();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeletePostAsyncWithAdmin(Guid id)
        {
            var deletePost = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (deletePost == null)
            {
                throw new Exception("Post not found");
            }
            _context.Posts.Remove(deletePost);
            _context.SaveChanges();
            return await Task.FromResult(true);
        }

        public async Task<List<PostDto>> GetAllAsync(int skip, int take)
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Block)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            var postdto = _mapper.Map<List<PostDto>>(post);
            return postdto;
        }

        public async Task<PostDto> GetByIdAsync(Guid id)
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Block)
                .FirstOrDefaultAsync(post => post.Id == id);
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            else
            {
                post.Views++;
                _context.SaveChanges();
                var postdto = _mapper.Map<PostDto>(post);
                return postdto;
            }
        }

        public async Task<List<PostDto>> GetByNamePostAsync(string name, int skip, int take)
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Block)
                .Where(post => post.Title.Contains(name))
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            if (post == null) {
                throw new Exception("Post not found");
            }
            var postdto = _mapper.Map<List<PostDto>>(post);
            return postdto;
        }

        public async Task<List<PostDto>> GetPostsByUserIdAsync(Guid userId, int skip, int take)
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Block)
                .Where(post => post.AuthorId == userId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            if (post == null) {
                throw new Exception("Post not found");
            }
            var postdto = _mapper.Map<List<PostDto>>(post);
            return postdto;
        }

        public async Task<List<PostDto>> GetPostWithAuthor(Guid userId, int skip, int take)
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Block)
                .Where(post => post.AuthorId == userId)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            var postdto = _mapper.Map<List<PostDto>>(post);
            return postdto;
        }

        public async Task<PostDto> UpdatePostAsync(Guid id, CreatePostDto dto, string coverImage, Guid userId)
        {
            var postId = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (postId == null)
            {
                throw new Exception("Post not found");
            }
            postId.Slug = SlugHelper.GenerateSlug(dto.Title);
            postId.Title = dto.Title;
            postId.Summary = dto.Summary;
            postId.CategoryId = dto.CategoryId;
            postId.CoverImage = coverImage;
            postId.ReadingMinutes = dto.ReadingMinutes;
            postId.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
            var postDto = _mapper.Map<PostDto>(postId);
            return await Task.FromResult(postDto);
        }
    }
}
