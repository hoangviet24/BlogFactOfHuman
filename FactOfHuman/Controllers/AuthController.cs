using FactOfHuman.Data;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.Token;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Enum;
using FactOfHuman.Extensions;
using FactOfHuman.Models;
using FactOfHuman.Repository.IService;
using FactOfHuman.Response;
using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
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
        private readonly IFileSerivce _fileService;
        public AuthController(IAuthService authService, FactOfHumanDbContext dbContext,IEmailService emailService, IWebHostEnvironment env, IFileSerivce fileService)
        {
            _authService = authService;
            _context = dbContext;
            _emailService = emailService;
            _env = env;
            _fileService = fileService;
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
            catch (UserNotActiveException ex)
            {
                return StatusCode(403, new ApiResponse<TokenResponseDto>(false, ex.Message));
            }
            catch (InvalidCredentialException ex)
            {
                return Unauthorized(new ApiResponse<TokenResponseDto>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<TokenResponseDto>(false, "Lỗi hệ thống: " + ex.Message));
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
                var result = await _authService.RefreshTokenAsync(userId,request);
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
                userId = User.getUserId();
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
            string avatarUrl = null;
            // Lấy đường dẫn file cũ
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (adminUpdateUserDto.AvatarUrl != null && adminUpdateUserDto.AvatarUrl.Length > 0)
            {
                _fileService.DeleteFile(user.AvatarUrl);
                avatarUrl = _fileService.SaveFile(adminUpdateUserDto.AvatarUrl, "uploads");
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
        public async Task<ActionResult<UserDto>> UpdateUser([FromForm] UpdateUserDto updateUserDto,[FromQuery] Role role )
        {
            var userId = User.getUserId();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Invalid or missing user ID in token" });
            }
            string avatarUrl = null;
            // Lấy đường dẫn file cũ
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (updateUserDto.AvatarUrl != null && updateUserDto.AvatarUrl.Length > 0)
            {
                _fileService.DeleteFile(user.AvatarUrl);
                avatarUrl = _fileService.SaveFile(updateUserDto.AvatarUrl, "uploads");
            }
            try
            {
                var updatedUser = await _authService.UpdateUser(userId!.Value, updateUserDto,avatarUrl,role);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            return Redirect($"https://playful-crumble-00ea56.netlify.app/login");
            //return Redirect($"http://localhost:5173/login");
            //return Ok(value: new { message = "Kích hoạt tài khoản thành công, vui lòng đăng nhập" });
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
            //return Redirect($"https://fact-of-human.web.app/");
            return Ok(value: new { message = "Gửi lại thành công, vui lòng kích hoạt lại" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get-all-user")]
        public async Task<IActionResult>GetAllUser(int skip = 0, int take = 30)
        {
            return Ok(await _authService.GetAllUser(skip,take));
        }
        [Authorize]
        [HttpPut("Change-Password")]
        public async Task<ActionResult> ChangePassword([FromBody]ChangePasswordDto dto)
        {
            var userId = User.getUserId();
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
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            Guid targetUserId = userId;
            if (userId == Guid.Empty)
            {
                var currentUserId = User.getUserId();
                if (currentUserId == null)
                    return Unauthorized(new { Message = "Invalid or missing user ID in token" });
                targetUserId = currentUserId.Value;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetUserId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            _fileService.DeleteFile(user.AvatarUrl);

            var result = await _authService.DeleteUser(targetUserId);
            return Ok(result);
        }
    }
}
