using api_template.Middleware.Wrappers;
using api_template.Models.User;

namespace api_template.Services.Auth
{
    public interface IAuthManager
    {
        Task<ApiResponse> RegisterAsync(CreateUserDto request);

        Task<ApiResponse> LoginAsync(UserDto request);

        Task<ApiResponse> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
