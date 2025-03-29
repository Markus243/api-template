using Microsoft.AspNetCore.Mvc;
namespace api_template.Attributes
{
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(string permission)
            : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }
}
