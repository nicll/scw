using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IAuthProcedures
{
    /// <summary>
    /// Retrieve user by their ID for authentication.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>Either the user object or <see langword="null"/>.</returns>
    Task<User?> GetUserById(Guid id);
}
