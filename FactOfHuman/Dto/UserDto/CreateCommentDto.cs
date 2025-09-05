using FactOfHuman.Models;
namespace FactOfHuman.Dto.UserDto
{
    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid? PostId { get; set; }
        public Guid? FactId { get; set; }
    }
}
