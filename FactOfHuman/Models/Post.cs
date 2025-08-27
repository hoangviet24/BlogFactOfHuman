using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public StatusPost Status { get; set; } = StatusPost.Draft; // Draft | Published | Archived
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public List<Tag> Tags { get; set; } = new();
        public string CoverImage { get; set; } = string.Empty;

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int Views { get; set; } = 0;
        public int ReadingMinutes { get; set; } = 0;
    }
}
