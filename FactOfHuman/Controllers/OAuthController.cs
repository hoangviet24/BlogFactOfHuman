using FactOfHuman.Dto.OAuth2;
using FactOfHuman.Dto.OAuth2.Facebook;
using FactOfHuman.Repository.IService;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FactOfHuman.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);

                var user = await _authService.GetOrCreateUserFromOAuth(
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
        [HttpPost("facebook")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginDto dto)
        {
            try
            {
                using var httpClient = new HttpClient();
                var url = $"https://graph.facebook.com/me?fields=id,name,email,picture.width(2048).height(2048)&access_token={dto.AccessToken}";
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return BadRequest("Invalid Facebook token");

                var content = await response.Content.ReadAsStringAsync();
                var fbUser = JsonSerializer.Deserialize<FacebookUserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

                var user = await _authService.GetOrCreateUserFromOAuth(
                    fbUser!.Email,
                    fbUser.Name,
                    fbUser.Picture.Data.Url
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
