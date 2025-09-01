namespace FactOfHuman.Dto.AuthDto
{
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public IFormFile? AvatarUrl { get; set; }
    }
}
