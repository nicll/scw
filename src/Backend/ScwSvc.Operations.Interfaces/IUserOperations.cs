using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Operations.Interfaces;

public interface IUserOperations
{
    /// <summary>
    /// Get a collection of all users.
    /// </summary>
    /// <returns>Collection of all users.</returns>
    Task<ICollection<User>> GetUsers();

    /// <summary>
    /// Get a user by ID.
    /// Alternatively return <see langword="null"/> if not found.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The user object or <see langword="null"/>.</returns>
    Task<User?> GetUserById(Guid userId);

    /// <summary>
    /// Get a user by name.
    /// Alternatively return <see langword="null"/> if not found.
    /// </summary>
    /// <param name="name">The user's name.</param>
    /// <returns>The user object or <see langword="null"/>.</returns>
    Task<User?> GetUserByName(string name);

    /// <summary>
    /// Modify one or more parts of a user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="name">The optional new name of the user.</param>
    /// <param name="password">The optional new password of the user.</param>
    /// <param name="role">The optional new role of the user.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name as the new one already exists.</exception>
    /// <exception cref="UserModificationException">Thrown if an invalid change was made.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task ModifyUser(Guid userId, string? name, string? password, UserRole? role);

    /// <summary>
    /// Add a user.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>The new user's ID.</returns>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name already exists.</exception>
    /// <exception cref="UserCredentialsInvalidException">Thrown if the supplied credentials are invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task<Guid> AddUser(string name, string password);

    /// <summary>
    /// Remove a user.
    /// </summary>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task DeleteUser(Guid userId);

    /// <summary>
    /// Attempt to log a user in.
    /// </summary>
    /// <param name="name">Name of the user.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>
    /// The user object if the credentials were correct.
    /// <see langword="null"/> if the expected and actual passwords are mismatched.
    /// </returns>
    /// <exception cref="UserNotFoundException">Thrown if no user with the supplied name exists.</exception>
    Task<User?> LoginUser(string name, string password);
}
