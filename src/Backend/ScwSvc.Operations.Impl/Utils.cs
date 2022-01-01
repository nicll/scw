using System.Security.Cryptography;
using System.Text;

namespace ScwSvc.Operations.Impl;

internal static class Utils
{
    /// <summary>
    /// The pepper used by the backend.
    /// </summary>
    private const string Pepper = "scw-";

    /// <summary>
    /// Hashes a users's password.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <param name="pass">Password of the user.</param>
    /// <returns>Hashed password of the user.</returns>
    internal static byte[] HashUserPassword(Guid userId, in string pass)
    {
        var hasher = SHA256.Create();
        var combination = Encoding.UTF8.GetBytes(Pepper + userId.ToDbName() + pass);
        return hasher.ComputeHash(combination);
    }

    /// <summary>
    /// Turns a <see cref="Guid"/> into a <see cref="String"/> that can be used as a name.
    /// </summary>
    /// <param name="guid">The given <see cref="Guid"/>.</param>
    /// <returns>The ID as a normalized <see cref="String"/>.</returns>
    internal static string ToDbName(this Guid guid)
        => guid.ToString("N");

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
