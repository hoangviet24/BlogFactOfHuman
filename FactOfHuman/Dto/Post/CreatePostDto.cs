using FactOfHuman.Enum;

namespace FactOfHuman.Dto.Post
{
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<Guid> Tags { get; set; } = new List<Guid>();
        public Guid CategoryId { get; set; }
        public IFormFile? CoverImage { get; set; }
        public int ReadingMinutes { get; set; }
    }
}
