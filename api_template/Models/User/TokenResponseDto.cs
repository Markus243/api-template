namespace api_template.Models.User
{
    public class TokenResponseDto
    {
        public long Id { get; set; }  //User Id
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string UserName { get; set; } = null!; 
        public string FirstName { get; set; } = null!; 
        public string LastName { get; set; } = null!;
    }
}
