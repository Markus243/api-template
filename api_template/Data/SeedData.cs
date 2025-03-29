using api_template.Data.Entities;
using Microsoft.EntityFrameworkCore;
using api_template.Data.Core;

namespace api_template.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAndPermissions(AppDbContext context)
        {
            // Define roles and permissions to seed
            var rolesToSeed = AvailableUserRoles.GetAll();
            var permissionsToSeed = new[] { "Create", "Update", "Delete", "Read" };

            // Seed roles
            foreach (var roleName in rolesToSeed)
            {
                var existingRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (existingRole == null)
                {
                    context.Roles.Add(new Role { Name = roleName });
                }
            }
            await context.SaveChangesAsync();

            // Seed permissions
            foreach (var permName in permissionsToSeed)
            {
                var existingPerm = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                if (existingPerm == null)
                {
                    context.Permissions.Add(new Permission { Name = permName });
                }
            }
            await context.SaveChangesAsync();

            // Assign permissions to Admin role
            var adminRole = await context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == AvailableUserRoles.Admin);

            if (adminRole != null)
            {
                foreach (var permName in permissionsToSeed)
                {
                    var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                    if (permission != null &&
                        !adminRole.RolePermissions.Any(rp => rp.PermissionId == permission.Id))
                    {
                        adminRole.RolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id
                        });
                    }
                }
            }
            await context.SaveChangesAsync();

            // Seed the base user - Markus
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "markus99");
            if (existingUser == null)
            {
                var user = new User
                {
                    Username = "markus99",
                    Email = "markusjmulder@gmail.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEIdrpwRVc+YOOc/pzw8hX/L6MMRN3wQyjq2jn1a2mZ5zOtGo4W0QrVtH+TlHMZHkfg==",
                    FirstName = "Markus",
                    LastName = "Mulder",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                // Assign Admin role to Markus
                var adminRoleEntity = await context.Roles.FirstOrDefaultAsync(r => r.Name == AvailableUserRoles.Admin);
                if (adminRoleEntity != null)
                {
                    context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = adminRoleEntity.Id
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
