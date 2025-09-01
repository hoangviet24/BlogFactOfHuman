using FactOfHuman.Enum;

namespace FactOfHuman.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AvatarUrl { get; set; } = string.Empty ;
        public string Bio { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Reader; // Reader | Contributor | Admin
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local; // Local | Google | Facebook
        public string activeToken { get; set; } = string.Empty;
        public DateTime ActiveTokenExpireAt { get; set; } = DateTime.UtcNow;
        public bool isActive { get; set; } = false;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
