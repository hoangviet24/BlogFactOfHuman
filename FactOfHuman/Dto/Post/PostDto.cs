namespace FactOfHuman.Dto.Post
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<PostBlockDto> Content { get; set; } = new List<PostBlockDto>();
        public string Status { get; set; } = string.Empty;
        public string? AuthorId { get; set; }
        public string? CategoryId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string CoverImage { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Views {  get; set; } 
        public int ReadingMinutes { get; set; }
    }
}
