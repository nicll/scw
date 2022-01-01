namespace ScwSvc.Operations.Impl;

public class UserOperations : IUserOperations
{
    private readonly ISysDbRepository _sysDb;
    private readonly IDynDbRepository _dynDb;

    public UserOperations(ISysDbRepository sysDb, IDynDbRepository dynDb)
    {
        _sysDb = sysDb;
        _dynDb = dynDb;
    }

    public async Task<Guid> AddUser(string name, string password)
    {
        if (await _sysDb.IsUserNameAssigned(name))
            throw new UserAlreadyExistsException("A user with this name already exists.");

        EnsureUserNameValid(name);
        EnsureUserPasswordValid(password);

        var userId = Guid.NewGuid();
        await _sysDb.AddUser(new()
        {
            UserId = userId,
            Name = name,
            PasswordHash = HashUserPassword(userId, password),
            Role = UserRole.Common,
            CreationDate = DateTime.UtcNow
        });

        await _sysDb.SaveChanges();
        return userId;
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _sysDb.GetUserById(id);

        if (user is null)
            throw new UserNotFoundException("Could not find user to delete.") { UserId = id };

        foreach (var table in user.OwnTables)
            await _dynDb.RemoveTable(table);

        await _sysDb.RemoveUser(user);
    }

    public async Task<User?> GetUserById(Guid id)
        => await _sysDb.GetUserById(id);

    public async Task<User?> GetUserByName(string name)
        => await _sysDb.GetUserByName(name);

    public async Task<ICollection<User>> GetUsers()
        => await _sysDb.GetAllUsers();

    public async Task ModifyUser(Guid id, string? name, string? password, UserRole? role)
    {
        var user = await _sysDb.GetUserById(id);

        if (user is null)
            throw new UserNotFoundException("Could not find user to modify.") { UserId = id };

        if (name is not null)
        {
            try
            {
                EnsureUserNameValid(name);
            }
            catch (UserCredentialsInvalidException e)
            {
                throw new UserChangeException(e.Message) { UserId = id, OldValue = user.Name, NewValue = name };
            }

            user.Name = name;
        }

        if (password is not null)
        {
            try
            {
                EnsureUserPasswordValid(password);
            }
            catch (UserCredentialsInvalidException e)
            {
                throw new UserChangeException(e.Message) { UserId = id, OldValue = "(old password)", NewValue = "(new password)" };
            }

            user.PasswordHash = HashUserPassword(id, password);
        }

        if (role.HasValue && role != user.Role)
            user.Role = role.Value;

        await _sysDb.ModifyUser(user);
        await _sysDb.SaveChanges();
    }

    public async Task<User?> LoginUser(string name, string password)
    {
        var user = await GetUserByName(name) ?? throw new UserNotFoundException("A user with this username does not exist.");
        var pwHash = HashUserPassword(user.UserId, password);

        if (CompareHashes(user.PasswordHash, pwHash))
            return user;

        return null;
    }

    private static void EnsureUserNameValid(string name)
    {
        if (name.Length < 4)
            throw new UserCredentialsInvalidException("User name is too short.") { InvalidValue = name };

        if (name.Length > 20)
            throw new UserCredentialsInvalidException("User name is too long.") { InvalidValue = name };
    }

    private static void EnsureUserPasswordValid(string password)
    {
        if (password.Length < 4)
            throw new UserCredentialsInvalidException("User name is too short.") { InvalidValue = password };

        if (password.Length > 256)
            throw new UserCredentialsInvalidException("User name is too long.") { InvalidValue = password };
    }
}
