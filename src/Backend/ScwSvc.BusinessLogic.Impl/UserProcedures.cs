using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Impl;

public class UserProcedures : IUserProcedures
{
    private readonly IUserOperations _user;
    private readonly ITableOperations _table;

    public UserProcedures(IUserOperations user, ITableOperations table)
    {
        _user = user;
        _table = table;
    }

    public async Task ChangeUserName(User user, string name)
        => await _user.ModifyUser(user.UserId, name, null, null);

    public async Task ChangeUserPassword(User user, string password)
        => await _user.ModifyUser(user.UserId, null, password, null);

    public Task<ICollection<User>> GetCollaborators(User user, Guid tableId)
    {
        var table = user.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.");

        return Task.FromResult(table.Collaborators);
    }

    public async Task AddCollaborator(User owner, Guid tableId, Guid userId)
    {
        if (owner.OwnTables.Any(t => t.TableId == tableId))
            throw new TableNotFoundException("The table was not found in the user's tables.");

        var collaborator = await _user.GetUserById(userId);

        if (collaborator is null)
            throw new UserNotFoundException("Collaborator was not found.");

        await _table.AddCollaborator(tableId, collaborator);
        // ToDo: logging
    }

    public async Task RemoveCollaborator(User owner, Guid tableId, Guid userId)
    {
        if (owner.OwnTables.Any(t => t.TableId == tableId))
            throw new TableNotFoundException("The table was not found in the user's tables.");

        var collaborator = await _user.GetUserById(userId);

        if (collaborator is null)
            throw new UserNotFoundException("Collaborator was not found.");

        await _table.RemoveCollaborator(tableId, collaborator);
        // ToDo: logging
    }
}
