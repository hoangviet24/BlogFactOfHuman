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
        Task<UserDto> AdminUpdateUser(Guid userId,Role role, AdminUpdateUserDto adminUpdateUserDto,string avatarUrl);
        Task<UserDto> UpdateUser(Guid userId, UpdateUserDto updateUserDto, string avatarUrl);
        Task<string> ChangePassword(ChangePasswordDto changePasswordDto, Guid userId);
        string GenerateJwtToken(User user);
        Task<User> GetOrCreateUserFromOAuth(string email, string? name = null, string? avatarUrl = null);
        Task<List<UserDto>> GetAllUser(int skip, int take);
        Task<bool> DeleteUser(Guid userId);
    }
}
