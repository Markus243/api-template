using System.ComponentModel.DataAnnotations;

namespace api_template.Data.Entities
{
    public class Permission
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
