using FactOfHuman.Dto.OAuth2;
using FactOfHuman.Repository.IService;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);

                var user = await _authService.GetOrCreateUserFromGoogle(
                    payload.Email,
                    payload.Name,
                    payload.Picture
                );

                var token = _authService.GenerateJwtToken(user);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
