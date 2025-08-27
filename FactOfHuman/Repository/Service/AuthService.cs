using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Token;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FactOfHuman.Repository.Service
{
    public class AuthService(
        FactOfHumanDbContext _context,
        IMapper _mapper,
        IConfiguration configuration) : IAuthService
    {
        public Task<bool> ActivateAccount(string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AdminUpdateUser(Guid userId, AdminUpdateUserDto adminUpdateUserDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangePassword(Guid userId, ChangePasswordDto changePasswordDto)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> ChangePassword(ChangePasswordDto changePasswordDto, string email)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> GetCurrentUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<TokenResponseDto> Login(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email || u.Name == loginDto.Email);
            if (Equals(loginDto.Email, string.Empty) || Equals(loginDto.Password, string.Empty))
            {
                throw new Exception("Email and Password are required");
            }
            if (user == null)
            {
                throw new Exception("User or Email not found");
            }
            var result = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Password is incorrect");
            }
            
            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                Token = GenerateJwtToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId,string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetValue<string>("Jwt:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<UserDto> Register(RegisterDto registerDto)
        {
            if (_userNameExists(registerDto.Name))
            {
                throw new Exception("Username already exists");
            }
            if (_userEmailExists(registerDto.Email))
            {
                throw new Exception("Email already exists");
            }
            if(Equals(registerDto.Password, string.Empty))
            {
                throw new Exception("Password is required");
            }
            var user = new User();
            var passwordHash = new PasswordHasher<User>()
                .HashPassword(user, registerDto.Password);
            user = _mapper.Map<User>(registerDto);
            user.PasswordHash = passwordHash;
            _context.Users.Add(user);
            _context.SaveChanges();
            var userDto = _mapper.Map<UserDto>(user);
            return await Task.FromResult(userDto);
        }
        private bool _userEmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
        private bool _userNameExists(string name)
        {
            return _context.Users.Any(u => u.Name == name);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request, Guid userId)
        {
            var user = await ValidateRefreshTokenAsync(userId, request.RefreshToken);
            if(user is null)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        } 
    }
}
