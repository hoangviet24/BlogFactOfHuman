using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Token;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Models;

namespace FactOfHuman.Repository.IService
{
    public interface IAuthService
    {
        Task<UserDto> Register(RegisterDto registerDto);
        Task<TokenResponseDto> Login(LoginDto loginDto);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request, Guid userId);
        Task<bool> ActivateAccount(string token);
        Task<UserDto> GetCurrentUser(Guid userId);
        Task<bool> AdminUpdateUser(Guid userId, AdminUpdateUserDto adminUpdateUserDto);
        Task<bool> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto);
        Task<UserDto> ChangePassword(ChangePasswordDto changePasswordDto, string email);
    }
}
