namespace FactOfHuman.Dto.OAuth2.Facebook
{
    public class FacebookUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty ;
        public string Email { get; set; } = string.Empty;
        public FacebookPicture Picture { get; set; } = new FacebookPicture() ;
    }
}
