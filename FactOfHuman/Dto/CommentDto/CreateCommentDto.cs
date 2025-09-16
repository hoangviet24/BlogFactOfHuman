namespace FactOfHuman.Dto.CommentDto
{
    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public Guid PostId { get; set; } = new();
    }
}
