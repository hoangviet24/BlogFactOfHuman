using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class Fact
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<string> Sources { get; set; } = new();

        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public List<Tag> Tags { get; set; } = new();

        public StatusFact Status { get; set; } = StatusFact.Visible; // visible | hidden
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
