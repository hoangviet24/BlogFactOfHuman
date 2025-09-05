using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class PostBlock
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null;
        public string TopContent { get; set; } = string.Empty;
        public string BottomContent { get; set; } = string.Empty;
        public string TopImage {  get; set; } = string.Empty;   
        public string BottomImage { get; set;} = string.Empty;
    }
}
