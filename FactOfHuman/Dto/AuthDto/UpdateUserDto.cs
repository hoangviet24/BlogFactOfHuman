namespace FactOfHuman.Dto.AuthDto
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public IFormFile? AvatarUrl { get; set; }
    }
}
