using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace api_template.Data.Entities
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email))]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        public string? Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;
        public string? AvatarFileName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
