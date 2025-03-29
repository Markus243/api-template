using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_template.Middleware.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected long UserId
        {
            get
            {
                // Retrieve the NameIdentifier claim from the user's claims
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Check if the claim exists and is not empty
                if (string.IsNullOrEmpty(userIdString))
                {
                    throw new UnauthorizedAccessException("User ID not found in token.");
                }

                // Attempt to parse the claim value to a long
                if (!long.TryParse(userIdString, out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid User ID format.");
                }

                return userId;
            }
        }
    }
}
