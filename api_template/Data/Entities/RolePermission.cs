using System.Security;

namespace api_template.Data.Entities
{
    public class RolePermission
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }
        public virtual Role Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}
