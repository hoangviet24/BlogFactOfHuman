namespace FactOfHuman.Dto.Post
{
    public class UpdatePostBlockDto
    {
        public string? TopContent { get; set; }
        public string? BottomContent { get; set; }
        public IFormFile? TopImageUrl { get; set; }
        public IFormFile? BottomImageUrl { get; set; }
    }
}
