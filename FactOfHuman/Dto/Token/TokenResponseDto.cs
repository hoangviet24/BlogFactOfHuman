namespace FactOfHuman.Dto.Token
{
    public class TokenResponseDto
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
