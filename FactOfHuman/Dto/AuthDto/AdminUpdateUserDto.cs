using FactOfHuman.Enum;

namespace FactOfHuman.Dto.AuthDto
{
    public class AdminUpdateUserDto
    {
        public string? UserName { get; set; }
        public string? Bio { get; set; }
        public IFormFile? AvatarUrl { get; set; }
    }
}
