using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public StatusComment Status { get; set; } = StatusComment.Visible; // visible | hidden
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Comment cho Post hoặc Fact
        public Guid? PostId { get; set; }
        public Post? Post { get; set; }

        public Guid? FactId { get; set; }
    }
}
