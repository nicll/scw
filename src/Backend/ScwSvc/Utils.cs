using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.Procedures.Interfaces;
using ScwSvc.SvcModels;

namespace ScwSvc;

internal static class Utils
{
    internal static class Configuration
    {
        /// <summary>
        /// Tries to get an environment variable.
        /// If the environment variable cannot be found an <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <param name="name">Name of the environment variable.</param>
        /// <returns>The value of the variable or an exception.</returns>
        /// <exception cref="ArgumentException">Thrown if the variable is not set.</exception>
        internal static string GetEnvironmentVariableOrFail(string name)
            => Environment.GetEnvironmentVariable(name) ?? throw new ArgumentException("Environment variable '" + name + "' not set.");

        /// <summary>
        /// Tries to get an environment variable.
        /// If the environment variable cannot be found <see langword="null"/> is returned.
        /// </summary>
        /// <param name="name">Name of the environment variable.</param>
        /// <returns>The value of the variable or <see langword="null"/>.</returns>
        internal static string? GetEnvironmentVariableOrNull(string name)
            => Environment.GetEnvironmentVariable(name);
    }

    internal static class Authentication
    {
        /// <summary>
        /// Tries to get the user ID as a <see cref="String"/>.
        /// Returns <see langword="null"/> if not defined.
        /// </summary>
        /// <param name="user">The "User" object.</param>
        /// <returns>The user ID or <see langword="null"/>.</returns>
        internal static string? GetUserIdAsStringOrNull(ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>
        /// Tries to get the user ID as a <see cref="Guid"/>.
        /// Returns <see langword="null"/> if not defined or invalid.
        /// </summary>
        /// <param name="user">The "User" object.</param>
        /// <returns>The user ID or <see langword="null"/>.</returns>
        internal static Guid? GetUserIdAsGuidOrNull(ClaimsPrincipal user)
        {
            try
            {
                var owner = GetUserIdAsStringOrNull(user);

                if (owner is null)
                    return null;

                return Guid.Parse(owner);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to get the user ID as a <see cref="String"/> and a <see cref="Guid"/>.
        /// Returns <see langword="null"/> if not defined or invalid.
        /// </summary>
        /// <param name="user">The "User" object.</param>
        /// <returns>The user ID or <see langword="null"/>.</returns>
        internal static (string idStr, Guid id)? GetUserIdAsGuidAndStringOrNull(ClaimsPrincipal user)
        {
            try
            {
                var owner = GetUserIdAsStringOrNull(user);

                if (owner is null)
                    return null;

                var ownerId = Guid.Parse(owner);
                return (owner, ownerId);
            }
            catch
            {
                return null;
            }
        }

        internal static async Task<SessionResult> GetUserOrError(IAuthProcedures authProc, ClaimsPrincipal cpUser)
        {
            var userInfo = Authentication.GetUserIdAsGuidOrNull(cpUser);

            if (!userInfo.HasValue)
                return SessionResult.Invalid(new UnauthorizedObjectResult("You are logged in with an invalid user."));

            var user = await authProc.GetUserById(userInfo.Value);

            if (user is null)
                return SessionResult.Invalid(new UnauthorizedObjectResult("You are logged in with a non-existent user."));

            return SessionResult.Valid(user);
        }

        internal static async Task<IActionResult> AuthenticateAndRun(IAuthProcedures authProc, ClaimsPrincipal cpUser, Func<User, Task<IActionResult>> code)
            => await GetUserOrError(authProc, cpUser) switch
            {
                SessionResult.InvalidSession error => error.Error,
                SessionResult.ValidSession user => await code(user.User),
                var result => throw new InvalidOperationException($"Invalid result from {nameof(GetUserOrError)}: {result?.GetType().Name}")
            };

        internal abstract record class SessionResult
        {
            private SessionResult() { }

            internal static ValidSession Valid(User user) => new ValidSession(user);

            internal static InvalidSession Invalid(IActionResult error) => new InvalidSession(error);

            internal record class ValidSession(User User) : SessionResult { }

            internal record class InvalidSession(IActionResult Error) : SessionResult { }
        }
    }

    internal static string ToSimplifiedFormat(this Guid userId)
        => userId.ToString("N");

    internal static bool AllUnique<T>(this IEnumerable<T> source)
    {
        var hs = new HashSet<T>();
        return source.All(hs.Add);
    }

    internal static ActionResult Forbidden(this ControllerBase controller, object value)
        => controller.Forbid(CookieAuthenticationDefaults.AuthenticationScheme);
    //=> controller.StatusCode(StatusCodes.Status403Forbidden, value);
}
