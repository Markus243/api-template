using System.Security.Claims;
using api_template.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace api_template.Attributes
{
    // Filter that checks if the user has the specified permission
    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly AppDbContext _context;

        public PermissionFilter(string permission, AppDbContext context)
        {
            _permission = permission;
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Retrieve the user's ID from claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!long.TryParse(userIdClaim.Value, out long userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if the user has the required permission through their roles
            var hasPermission = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .AnyAsync(rp => rp.Permission.Name == _permission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
