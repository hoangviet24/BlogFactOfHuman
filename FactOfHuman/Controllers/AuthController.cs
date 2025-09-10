using FactOfHuman.Data;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Token;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Enum;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly FactOfHumanDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        public AuthController(IAuthService authService, FactOfHumanDbContext dbContext,IEmailService emailService, IWebHostEnvironment env)
        {
            _authService = authService;
            _context = dbContext;
            _emailService = emailService;
            _env = env;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await _authService.Register(registerDto);
                return Ok(new { user, message = "Vui lòng kích hoạt tài khoản" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
         
            }
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID in token" });
            }
            try
            {
                var result = await _authService.RefreshTokenAsync(request,userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUser([FromQuery] Guid? userId)
        {
            if(userId == null)
            {
                userId = GetUserIdFromClaims();
            }
            if(userId == null)
            {
                return Unauthorized(new { Message = "Invalid or missing user ID in token" });
            }

            try
            {
                var user = await _authService.GetCurrentUser(userId!.Value);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving user information" });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("admin-update-user/{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UserDto>> AdminUpdateUser([FromRoute] Guid userId,[FromQuery] Role role, [FromForm] AdminUpdateUserDto adminUpdateUserDto)
        {
            string avatarUrl = string.Empty;
            // Lấy đường dẫn file cũ
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            string? oldAvatarPath = null;
            if (!string.IsNullOrEmpty(user?.AvatarUrl))
            {
                oldAvatarPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), user.AvatarUrl.TrimStart('/'));
            }

            if (adminUpdateUserDto.AvatarUrl != null && adminUpdateUserDto.AvatarUrl.Length > 0)
            {
                var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadFolder = Path.Combine(webrootPath, "uploads");
                Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(adminUpdateUserDto.AvatarUrl.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await adminUpdateUserDto.AvatarUrl.CopyToAsync(stream);
                }

                avatarUrl = $"/uploads/{fileName}";

                // Xóa file cũ
                if (!string.IsNullOrEmpty(oldAvatarPath) && System.IO.File.Exists(oldAvatarPath))
                {
                    try
                    {
                        System.IO.File.Delete(oldAvatarPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không xóa được ảnh cũ: {ex.Message}");
                    }
                }
            }

            try
            {
                var updatedUser = await _authService.AdminUpdateUser(userId,role, adminUpdateUserDto, avatarUrl);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("update-user")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UserDto>> UpdateUser([FromForm] UpdateUserDto updateUserDto )
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Invalid or missing user ID in token" });
            }
            string avatarUrl = string.Empty;
            // Lấy đường dẫn file cũ
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            string? oldAvatarPath = null;
            if (!string.IsNullOrEmpty(user?.AvatarUrl))
            {
                oldAvatarPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), user.AvatarUrl.TrimStart('/'));
            }

            if (updateUserDto.AvatarUrl != null && updateUserDto.AvatarUrl.Length > 0)
            {
                var webrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadFolder = Path.Combine(webrootPath, "uploads");
                Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(updateUserDto.AvatarUrl.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateUserDto.AvatarUrl.CopyToAsync(stream);
                }

                avatarUrl = $"/uploads/{fileName}";

                // Xóa file cũ
                if (!string.IsNullOrEmpty(oldAvatarPath) && System.IO.File.Exists(oldAvatarPath))
                {
                    try
                    {
                        System.IO.File.Delete(oldAvatarPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không xóa được ảnh cũ: {ex.Message}");
                    }
                }
            }

            try
            {
                var updatedUser = await _authService.UpdateUser(userId!.Value, updateUserDto,avatarUrl);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private Guid? GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }
            return userId;
        }
        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string email, [FromQuery(Name = "activeToken")] string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.activeToken == token);
            if (user == null) return BadRequest("Token không hợp lệ");
            if(user.ActiveTokenExpireAt < DateTime.UtcNow)
            {
                return BadRequest("Token đã hết hạn");
            }

            user.isActive = true;
            user.activeToken = ""; // clear
            await _context.SaveChangesAsync();

            //return Redirect($"https://fact-of-human.web.app/");
            return Ok(value: new { message = "Kích hoạt tài khoản thành công, vui lòng đăng nhập" });
        }
        [HttpPost("resend-email")]
        public async Task<IActionResult> ResendActive([FromQuery] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return BadRequest("Email không hợp lệ");
            if (user.isActive)
            {
                return Conflict("Email đã được kích hoạt" );
            }
            user.activeToken = Guid.NewGuid().ToString();
            user.ActiveTokenExpireAt = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();
            var activationLink = $"https://localhost:7051/api/Auth/activate?email={user.Email}&activeToken={user.activeToken}";
            var templatePath = Path.Combine(_env.ContentRootPath, "EmailTemplate", "activation.html");
            var html = await System.IO.File.ReadAllTextAsync(templatePath);
            html = html.Replace("{{Username}}", user.Name)
                .Replace("{{ActivationLink}}", activationLink);
            await _emailService.SendEmailAsync(email,"Active email",html);
            return Redirect($"https://fact-of-human.web.app/");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get-all-user")]
        public async Task<IActionResult>GetAllUser()
        {
            return Ok(await _authService.GetAllUser());
        }
        [Authorize]
        [HttpPut("Change-Password")]
        public async Task<ActionResult> ChangePassword([FromBody]ChangePasswordDto dto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized(new { Message = "Invalid or missing user ID in token" });
            try
            {
                var user = await _authService.ChangePassword(dto, userId.Value);
                return Ok(user);
            }
            catch (Exception ex) {
                return Ok(ex.Message);
            }   
        }
        [Authorize]
        [HttpDelete("Delete-User/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute]Guid userId)
        {
            if(userId == Guid.Empty)
            {
                var currentUserId = GetUserIdFromClaims();
                return Ok(await _authService.DeleteUser(currentUserId.Value));
            }
            return Ok(await _authService.DeleteUser(userId));
        }
    }
}
