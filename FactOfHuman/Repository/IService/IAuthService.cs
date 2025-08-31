using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Token;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Enum;
using FactOfHuman.Models;

namespace FactOfHuman.Repository.IService
{
    public interface IAuthService
    {
        Task<UserDto> Register(RegisterDto registerDto);
        Task<TokenResponseDto> Login(LoginDto loginDto);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request, Guid userId);
        Task<UserDto> GetCurrentUser(Guid userId);
        Task<UserDto> AdminUpdateUser(Guid userId,Role role, AdminUpdateUserDto adminUpdateUserDto);
        Task<UserDto> UpdateUser(Guid userId, UpdateUserDto updateUserDto);
        Task<bool> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto);
        Task<UserDto> ChangePassword(ChangePasswordDto changePasswordDto, string email);
        string GenerateJwtToken(User user);
        Task<User> GetOrCreateUserFromGoogle(string email, string? name = null, string? avatarUrl = null);
        Task<List<UserDto>> GetAllUser();
    }
}
