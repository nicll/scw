using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ScwSvc
{
    internal static class Utils
    {
        /// <summary>
        /// The pepper used by the backend.
        /// </summary>
        private const string Pepper = "scw-";

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
            internal static string GetEnvironmentVariableOrNull(string name)
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
            internal static string GetUserIdAsStringOrNull(ClaimsPrincipal user)
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
                    var ownerId = Guid.Parse(owner);
                    return ownerId;
                }
                catch
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
                    var ownerId = Guid.Parse(owner);
                    return (owner, ownerId);
                }
                catch
                {
                    return null;
                }
            }

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

        internal static class DataConversion
        {
            /// <summary>
            /// Converts the table reference ID and an array of <see cref="CreateDataSetModel.ColumnDefinition"/>
            /// to an array of <see cref="DataSetColumn"/>.
            /// </summary>
            /// <param name="definition">The column definitions.</param>
            /// <param name="tableRefId">The table reference ID.</param>
            /// <returns>The converted definitions.</returns>
            internal static DataSetColumn[] ConvertColumns(CreateDataSetModel.ColumnDefinition[] definition, Guid tableRefId)
            {
                if (definition.Length > Byte.MaxValue)
                    throw new InvalidTableException("Too many columns in table.");

                var hs = new HashSet<string>();

                if (!definition.Select(c => c.Name).AllUnique())
                    throw new InvalidTableException("Column names not unique.");

                var result = new DataSetColumn[definition.Length];

                for (int i = 0; i < definition.Length; ++i)
                {
                    result[i] = new DataSetColumn()
                    {
                        TableRefId = tableRefId,
                        Position = (byte)i,
                        Name = definition[i].Name,
                        Nullable = definition[i].Nullable,
                        Type = definition[i].Type
                    };
                }

                return result;
            }
        }

        internal static bool AllUnique<T>(this IEnumerable<T> source)
        {
            var hs = new HashSet<T>();
            return source.All(hs.Add);
        }

        /// <summary>
        /// Turns a <see cref="Guid"/> into a <see cref="String"/> that can be used as a name.
        /// </summary>
        /// <param name="guid">The given <see cref="Guid"/>.</param>
        /// <returns>The ID as a normalized <see cref="String"/>.</returns>
        internal static string ToNameString(this Guid guid)
            => guid.ToString("N");

        internal static ActionResult Forbidden(this ControllerBase controller, object value)
            => controller.Forbid(CookieAuthenticationDefaults.AuthenticationScheme);
        //=> controller.StatusCode(StatusCodes.Status403Forbidden, value);
    }
}
