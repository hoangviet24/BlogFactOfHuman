using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.Post;
using FactOfHuman.Enum;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace FactOfHuman.Repository.Service
{
    public class PostService : IPostService
    {
        private readonly FactOfHumanDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;   
        public PostService(FactOfHumanDbContext context, IMapper mapper, ICommentService commentService)
        {
            _context = context;
            _mapper = mapper;
            _commentService = commentService;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto,string coverImage, Guid userId)
        {
            var titleExists = await _context.Posts.AnyAsync(p => p.Title == dto.Title);
            if (titleExists)
            {
                throw new Exception("Title already exists");
            }
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
            const int batchSize = 500000;

            _context.Database.SetCommandTimeout(0); // đặt 0 = vô hạn

            while (true)
            {
                var deletedComments = await _context.Database.ExecuteSqlRawAsync(
                    "DELETE TOP(@p0) FROM Comments WHERE PostId = @p1",
                    batchSize, id);

                if (deletedComments == 0)
                    break;
            }

            var deletedPosts = await _context.Posts
                .Where(p => p.Id == id && p.AuthorId == userId)
                .ExecuteDeleteAsync();

            if (deletedPosts == 0)
                throw new Exception("Post not found");

            return true;
        }
        public async Task<bool> DeletePostAsyncWithAdmin(Guid id)
        {

            await _context.Comments.Where(c => c.PostId == id).ExecuteDeleteAsync();
            var deletePost = _context.Posts.Where(p => p.Id == id).ExecuteDeleteAsync();
            if (deletePost == null)
            {
                throw new Exception("Post not found");
            }
            return await Task.FromResult(true);
        }
        public async Task<List<PostDto>> GetAllAsync(int skip, int take)
        {
            var post = await _context.Posts
                .Include(post => post.Tags)
                .Include(post => post.Block)
                .Include(post => post.Author)
                .Include(post => post.Category)
                .OrderByDescending(post => post.Views)
                .ThenByDescending(post => post.PublishedAt)
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
                .OrderByDescending(post => post.Views)
                .ThenByDescending(post => post.PublishedAt)
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
                .OrderByDescending(post => post.Views)
                .ThenByDescending(post => post.PublishedAt)
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
                .OrderByDescending(post => post.Views)
                .ThenByDescending(post => post.PublishedAt)
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

        public async Task<PostDto> UpdatePostAsync(Guid id, StatusPost status, CreatePostDto dto, string coverImage, Guid userId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
                throw new Exception("Post not found");

            var validTagIds = dto.Tags?.Where(t => t.HasValue).Select(t => t.Value).ToList() ?? new List<Guid>();

            Console.WriteLine($"Valid Tags Count: {validTagIds.Count}");

            if (validTagIds.Count > 0)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM PostTag WHERE PostId = {0}", post.Id);

                var newTags = await _context.Tags
                    .Where(t => validTagIds.Contains(t.Id))
                    .ToListAsync();

                post.Tags = newTags;
            }
            post.Slug = SlugHelper.GenerateSlug(dto.Title);
            post.Title = dto.Title ?? post.Title;
            post.Summary = dto.Summary ?? post.Summary;
            post.Status = status;
            post.CategoryId = dto.CategoryId ?? post.CategoryId;
            post.CoverImage = coverImage ?? post.CoverImage;
            post.ReadingMinutes = dto.ReadingMinutes;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            var updatedPost = await _context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == post.Id);
            return _mapper.Map<PostDto>(updatedPost);
        }

    }
}
