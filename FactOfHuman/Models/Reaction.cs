using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class Reaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public TargetType TargetType { get; set; } = TargetType.Post; // post | fact | comment
        public Guid TargetId { get; set; }
        public TypeReaction Type { get; set; } = TypeReaction.Like; // like | love | insightful
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
