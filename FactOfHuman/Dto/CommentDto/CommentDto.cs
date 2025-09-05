using FactOfHuman.Enum;

namespace FactOfHuman.Dto.CommentDto
{
    public class CommentDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; 
        public string statusComment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } 
        public string PostName { get; set; } = string.Empty;
        public string FactName { get; set; } = string.Empty;
    }
}
