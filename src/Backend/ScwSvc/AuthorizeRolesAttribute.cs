using Microsoft.AspNetCore.Authorization;
using System;

namespace ScwSvc
{
    /// <summary>
    /// Allows access to all roles specified.
    /// </summary>
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}
