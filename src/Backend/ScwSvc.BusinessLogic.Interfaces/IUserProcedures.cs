using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IUserProcedures
{
    //Task AddUser(User user);

    //Task RemoveUser(User user);

    /// <summary>
    /// Change the name of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="name">The new name.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name as the new one already exists.</exception>
    /// <exception cref="UserChangeException">Thrown if the new name was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task ChangeUserName(User user, string name);

    /// <summary>
    /// Change the password of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The new password.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserChangeException">Thrown if the new password was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task ChangeUserPassword(User user, string password);

    //Task ChangeUserRole(User user, UserRole role);

    //Task<IEnumerable<TableRef>> GetTables();

    //Task<IEnumerable<TableRef>> GetUserTables(User user, TableQuery query);

    //Task<TableRef> GetTable(User user, Guid tableId);

    //Task CreateDataSet();

    //Task DeleteDataSet();

    //Task CreateSheet();

    //Task DeleteSheet();

    /// <summary>
    /// Get a collection of all collaborators of a table.
    /// </summary>
    /// <param name="user">The owner of the table.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>Collection of the collaborators.</returns>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found in the owner's tables.</exception>
    Task<ICollection<User>> GetCollaborators(User user, Guid tableId);

    /// <summary>
    /// Add a collaborator to a table.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the collaborator to add.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found in the owner's tables.</exception>
    /// <exception cref="UserNotFoundException">Thrown if the collaborator was not found.</exception>
    /// <exception cref="TableCollaboratorException">Thrown if the collaborator was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddCollaborator(User owner, Guid tableId, Guid userId);

    /// <summary>
    /// Remove a collaborator from a table.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the collaborator to remove.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found in the owner's tables.</exception>
    /// <exception cref="UserNotFoundException">Thrown if the collaborator was not found.</exception>
    /// <exception cref="TableCollaboratorException">Thrown if the collaborator was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task RemoveCollaborator(User owner, Guid tableId, Guid userId);
}
