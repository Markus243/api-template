using api_template.Data;
using api_template.Data.Entities;
using api_template.Middleware.Wrappers;
using api_template.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace api_template.Services.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthManager(AppDbContext dbContext, IConfiguration configuration)
        {
            _db = dbContext;
            _configuration = configuration;
        }

        // Register a new user, returns ApiResponse
        public async Task<ApiResponse> RegisterAsync(CreateUserDto request)
        {
            // Check if a user with the same username already exists
            if (await _db.Users.AnyAsync(u => u.Username == request.Username))
            {
                return new ApiResponse(Status400BadRequest, new ApiError("User with the same username already exists."));
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = new User();
                var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

                user.Username = request.Username;
                user.PasswordHash = hashedPassword;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                var roleEntity = await _db.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
                if (roleEntity != null)
                {
                    _db.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleEntity.Id
                    });
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return new ApiResponse(Status201Created, "User registered successfully");
            }
            catch
            {
                await transaction.RollbackAsync();
                return new ApiResponse(Status400BadRequest, "An error occurred while registering the user.");
            }
        }

        // Login a user, returns ApiResponse
        public async Task<ApiResponse> LoginAsync(UserDto request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return new ApiResponse(Status401Unauthorized, new ApiError("Invalid credentials."));
            }

            var passwordCheck = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (passwordCheck == PasswordVerificationResult.Failed)
            {
                return new ApiResponse(Status401Unauthorized, new ApiError("Invalid credentials."));
            }

            var tokenResponse = await CreateTokenResponse(user);

            return new ApiResponse(Status200OK, "User logged in successfully", tokenResponse);
        }

        // Refresh an existing token, returns ApiResponse
        public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user == null)
            {
                return new ApiResponse(Status401Unauthorized, new ApiError("Invalid refresh token."));
            }

            var tokenResponse = await CreateTokenResponse(user);
            return new ApiResponse(Status200OK, "Token refreshed successfully", tokenResponse);
        }

        #region HELPERS
        // Create the main JWT token
        private async Task<string> CreateTokenAsync(User user)
        {
            var userRoles = await _db.UserRoles
                .Include(x => x.Role)
                .Where(ur => ur.UserId == user.Id)
                .AsNoTracking()
                .ToListAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        // Helper to generate a new token/refresh combo
        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                Id = user.Id,
                Token = await CreateTokenAsync(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        // Generate and save refresh token
        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshtoken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _db.SaveChangesAsync();
            return refreshToken;
        }

        private static string GenerateRefreshtoken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        // Validate refresh token in database
        private async Task<User?> ValidateRefreshTokenAsync(long userId, string refreshToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
        #endregion
    }
}
