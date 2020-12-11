using System;
using System.Security.Claims;

namespace ScwSvc
{
    internal static class Utils
    {
        internal static string GetEnvironmentVariableOrFail(string name)
            => Environment.GetEnvironmentVariable(name) ?? throw new ArgumentException("Environment variable '" + name + "' not set.");

        internal static string GetEnvironmentVariableOrNull(string name)
            => Environment.GetEnvironmentVariable(name);

        internal static string GetUserIdAsStringOrNull(ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        internal static Guid? GetUserIdAsGuidOrNull(ClaimsPrincipal user)
        {
            try
            {
                var owner = GetUserIdAsStringOrNull(user);
                var ownerId = Guid.Parse(owner);
                return ownerId;
            }
            catch
            {
                return null;
            }
        }

        internal static (string idStr, Guid id)? GetUserIdAsGuidAndStringOrNull(ClaimsPrincipal user)
        {
            try
            {
                var owner = GetUserIdAsStringOrNull(user);
                var ownerId = Guid.Parse(owner);
                return (owner, ownerId);
            }
            catch
            {
                return null;
            }
        }

        internal static string ToNameString(this Guid guid)
            => guid.ToString("N");
    }
}
