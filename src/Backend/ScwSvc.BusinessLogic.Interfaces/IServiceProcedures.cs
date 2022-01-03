using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IServiceProcedures
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>The ID of the new user.</returns>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name already exists.</exception>
    /// <exception cref="UserCredentialsInvalidException">Thrown if the supplied credentials are invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task<Guid> RegisterUser(string name, string password);

    /// <summary>
    /// Try to login as a new user.
    /// </summary>
    /// <param name="username">The name of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>The user object if credentials correct or <see langword="null"/> if mismatched.</returns>
    /// <exception cref="UserNotFoundException">Thrown if no user with the supplied name exists.</exception>
    Task<User?> LoginUser(string username, string password);
}
