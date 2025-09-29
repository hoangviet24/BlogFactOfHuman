using FactOfHuman.Enum;
using Microsoft.AspNetCore.Mvc;

namespace FactOfHuman.Dto.Post
{
    public class CreatePostDto
    {
        public string? Title { get; set; } 
        public string? Summary { get; set; } 
        public List<Guid?> Tags { get; set; }
        public Guid? CategoryId { get; set; }
        [FromForm]
        public IFormFile? CoverImage { get; set; }
        public int ReadingMinutes { get; set; }
    }
}
