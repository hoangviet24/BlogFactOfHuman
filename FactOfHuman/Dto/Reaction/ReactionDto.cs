using FactOfHuman.Enum;
using FactOfHuman.Models;

namespace FactOfHuman.Dto.Reaction
{
    public class ReactionDto
    {
        public Guid Id { get; set; }
        public String Username { get; set; } =string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
