using Microsoft.AspNetCore.Authorization;

namespace WebApiShop.Authorization
{
    /// <summary>
    /// Custom authorize attribute that restricts access by role.
    /// Usage: [AuthorizeRole(Roles.Admin)] or [AuthorizeRole(Roles.Admin, Roles.User)].
    /// </summary>
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public AuthorizeRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
