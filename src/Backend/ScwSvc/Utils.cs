using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ScwSvc
{
    internal static class Utils
    {
        private const string Pepper = "scw-";

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

        internal static ActionResult Forbidden(this ControllerBase controller, object value)
            => controller.Forbid(CookieAuthenticationDefaults.AuthenticationScheme);
        //=> controller.StatusCode(StatusCodes.Status403Forbidden, value);

        /// <summary>
        /// Hashes a users's password.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="pass">Password of the user.</param>
        /// <returns>Hashed password of the user.</returns>
        internal static byte[] HashUserPassword(Guid userId, in string pass)
        {
            var hasher = SHA256.Create();
            var combination = Encoding.UTF8.GetBytes(Pepper + userId.ToNameString() + pass);
            return hasher.ComputeHash(combination);
        }

        /// <summary>
        /// Compare two memory areas for equal content.
        /// Basically like memcmp().
        /// </summary>
        /// <param name="left">First memory area.</param>
        /// <param name="right">Second memory area.</param>
        /// <returns>Whether or not the two areas contain the same content.</returns>
        internal static bool CompareHashes(in ReadOnlySpan<byte> left, in ReadOnlySpan<byte> right)
            => left.SequenceEqual(right);
    }
}
