using FactOfHuman.Enum;

namespace FactOfHuman.Dto.AuthDto
{
    public class AdminUpdateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public IFormFile? AvatarUrl { get; set; }
    }
}
