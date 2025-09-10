using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class Bookmark
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public TargetType TargetType { get; set; } = TargetType.Post; // Post | Fact
        public Guid TargetId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
