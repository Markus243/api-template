using System.ComponentModel.DataAnnotations;

namespace api_template.Data.Entities
{
    public class Role
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
