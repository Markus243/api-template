namespace api_template.Models.User
{
    public class RefreshTokenRequestDto
    {
        public long UserId { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
