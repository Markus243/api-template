using System.ComponentModel.DataAnnotations;
using api_template.Data.Core;

namespace api_template.Models.User
{
    public class CreateUserDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [RegularExpression(AvailableUserRoles.AllowedTypes, ErrorMessage = "Only allowed user roles are allowed.")]
        public string Role { get; set; } = null!;
    }
}
