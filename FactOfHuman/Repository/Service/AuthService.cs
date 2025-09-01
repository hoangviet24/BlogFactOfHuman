using AutoMapper;
using FactOfHuman.Data;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Token;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Enum;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FactOfHuman.Repository.Service
{
    public class AuthService(
        FactOfHumanDbContext _context,
        IMapper _mapper,
        IConfiguration configuration,
        IEmailService emailService,
        IWebHostEnvironment _env) : IAuthService
    {
        public async Task<UserDto> AdminUpdateUser(Guid userId,Role role, AdminUpdateUserDto adminUpdateUserDto,string avatarUrl)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }
            else
            {
                if(user.AuthProvider == AuthProvider.Google)
                {
                    user.Role = role;
                }
                else
                {
                    user.Name = adminUpdateUserDto.UserName ?? user.Name;
                    user.AvatarUrl = avatarUrl ?? user.AvatarUrl;
                    user.Bio = adminUpdateUserDto.Bio ?? user.Bio;
                    user.Role = role;
                }
                await _context.SaveChangesAsync();
            }
            var userDto = _mapper.Map<UserDto>(user);   
            return await Task.FromResult(userDto);
        }

        public async Task<string> ChangePassword(ChangePasswordDto changePasswordDto, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) {
                throw new BadHttpRequestException("User not found");
            }
            var passwordCheck = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, changePasswordDto.CurrentPassword);
            if (Equals(changePasswordDto.CurrentPassword, string.Empty) || Equals(changePasswordDto.NewPassword, string.Empty))
            {
                throw new BadHttpRequestException("Field Password is not empty");
            }
            if (passwordCheck == PasswordVerificationResult.Failed)
            {
                throw new BadHttpRequestException("Password is Incorrect");
            }
            if(changePasswordDto.CurrentPassword == changePasswordDto.NewPassword)
            {
                throw new BadHttpRequestException("New password cannot be the same as the current password");
            }
            var newPassword = new PasswordHasher<User>().HashPassword(user,changePasswordDto.NewPassword);
            user.PasswordHash = newPassword;
            await _context.SaveChangesAsync();
            return "Change Success";
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
            var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == loginDto.Email || u.Name == loginDto.Email);
            
            if (Equals(loginDto.Email, string.Empty) || Equals(loginDto.Password, string.Empty))
            {
                throw new BadHttpRequestException("Email and Password are required");
            }
            if (user == null)
            {
                throw new BadHttpRequestException("User or Email not found");
            }
            var result = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadHttpRequestException("Password is incorrect");
            }
            if (user!.isActive == false)
            {
                throw new BadHttpRequestException("Email is not active");
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
        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("auth_provider", user.AuthProvider.ToString()),
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
            user.activeToken = Guid.NewGuid().ToString();
            user.isActive = false; // cần active account
            user.ActiveTokenExpireAt = DateTime.UtcNow.AddHours(24);
            _context.Users.Add(user);
            _context.SaveChanges();
            var activationLink = $"https://localhost:7051/api/Auth/activate?email={user.Email}&activeToken={user.activeToken}";
            var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplate","activation.html");
            var html = await File.ReadAllTextAsync(templatePath);
            html = html.Replace("{{Username}}", user.Name)
                .Replace("{{ActivationLink}}", activationLink);
            await emailService.SendEmailAsync(user.Email, "Active account", html);
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

        public async Task<User> GetOrCreateUserFromOAuth(string email, string? name = null, string? avatarUrl = null)
        {
            // 1. Kiểm tra user đã tồn tại
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                // Cập nhật info nếu muốn
                user.Name = name ?? user.Name;
                user.AvatarUrl = avatarUrl ?? user.AvatarUrl;
                await _context.SaveChangesAsync();
                return user;
            }

            // 2. Tạo user mới
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name ?? "Google User",
                AvatarUrl = avatarUrl ?? string.Empty,
                isActive = true, // google login -> active luôn
                CreatedAt = DateTime.UtcNow, 
                Role = Role.Reader,
                AuthProvider = AuthProvider.Google,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<UserDto> UpdateUser(Guid userId, UpdateUserDto updateUserDto,string avatarUrl)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if(user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }
            else
            {
                if(user.AuthProvider == AuthProvider.Google)
                {
                    user.Bio = updateUserDto.Bio ?? user.Bio;
                }
                else
                {
                    user.Name = updateUserDto.Name ?? user.Name;
                    user.AvatarUrl = avatarUrl ?? user.AvatarUrl;
                    user.Bio = updateUserDto.Bio ?? user.Bio;
                }
                await _context.SaveChangesAsync();
            }
            var userDto = _mapper.Map<UserDto>(user);   
            return await Task.FromResult(userDto);
        }

        public async Task<List<UserDto>> GetAllUser()
        {
            var user = await _context.Users.ToListAsync();
            var useDto = _mapper.Map<List<UserDto>>(user);
            return useDto;
        }
    }
}
