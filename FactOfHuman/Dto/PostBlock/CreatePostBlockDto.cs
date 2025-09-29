using FactOfHuman.Enum;

namespace FactOfHuman.Dto.PostBlock
{
    public class CreatePostBlockDto
    {
        public Guid PostId { get; set; }
        public string? TopContent { get; set; } 
        public string? BottomContent { get; set; }
        public IFormFile? TopImageUrl { get; set; }
        public IFormFile? BottomImageUrl { get; set; }
    }
}
