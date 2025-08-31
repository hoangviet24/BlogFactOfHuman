namespace FactOfHuman.Dto.UserDto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string AuthProvider { get; set; } = string.Empty;
        public bool isActive { get; set; }
    }
}
