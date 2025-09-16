using FactOfHuman.Enum;

namespace FactOfHuman.Dto.Post
{
    public class CreatePostDto
    {
        public string? Title { get; set; } 
        public string? Summary { get; set; } 
        public List<Guid?> Tags { get; set; }
        public Guid? CategoryId { get; set; }
        public IFormFile? CoverImage { get; set; }
        public int ReadingMinutes { get; set; }
    }
}
