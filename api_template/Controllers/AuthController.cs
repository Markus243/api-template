using api_template.Middleware.Wrappers;
using api_template.Models.User;
using api_template.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace api_template.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authService;

        public AuthController(IAuthManager authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ApiResponse> Register(CreateUserDto request)
        {
            var response = await _authService.RegisterAsync(request);
            return response;
        }

        [HttpPost("login")]
        public async Task<ApiResponse> Login(UserDto request)
        {
            var response = await _authService.LoginAsync(request);
            return response;
        }

        [HttpPost("refresh-token")]
        public async Task<ApiResponse> RefreshToken(RefreshTokenRequestDto request)
        {
            var response = await _authService.RefreshTokenAsync(request);
            return response;
        }
    }
}
