using System.Linq;

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
        await EnsureUserNameValid(name);
        EnsureUserPasswordValid(password);

        var userId = Guid.NewGuid();
        await _sysDb.AddUser(new()
        {
            UserId = userId,
            Name = name,
            PasswordHash = HashUserPassword(userId, password),
            Role = UserRole.Common,
            CreationDate = DateTime.UtcNow,
            OwnTables = Array.Empty<Table>(),
            Collaborations = Array.Empty<Table>()
        });

        await _sysDb.SaveChanges();
        return userId;
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await _sysDb.GetUserById(userId);

        if (user is null)
            throw new UserNotFoundException("Could not find user to delete.") { UserId = userId };

        foreach (var table in user.OwnTables)
            await _dynDb.RemoveTable(table);

        await _sysDb.RemoveUser(user);
    }

    public async Task<User?> GetUserById(Guid userId)
        => await _sysDb.GetUserById(userId);

    public async Task<User?> GetUserByName(string name)
        => await _sysDb.GetUserByName(name);

    public async Task<ICollection<User>> GetUsers()
        => await _sysDb.GetAllUsers();

    public async Task ModifyUser(Guid userId, string? name, string? password, UserRole? role)
    {
        var user = await _sysDb.GetUserById(userId);

        if (user is null)
            throw new UserNotFoundException("Could not find user to modify.") { UserId = userId };

        if (name is not null)
        {
            try
            {
                await EnsureUserNameValid(name);
            }
            catch (UserCredentialsInvalidException e)
            {
                throw new UserModificationException(e.Message) { UserId = userId, OldValue = user.Name, NewValue = name };
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
                throw new UserModificationException(e.Message) { UserId = userId, OldValue = "(old password)", NewValue = "(new password)" };
            }

            user.PasswordHash = HashUserPassword(userId, password);
        }

        if (role.HasValue && role != user.Role)
        {
            if (!Enum.IsDefined(role.Value))
                throw new UserModificationException("The new role is unknown.") { OldValue = user.Role, NewValue = role };

            user.Role = role.Value;
        }

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

    private async Task EnsureUserNameValid(string name)
    {
        if (name.Length < 4)
            throw new UserCredentialsInvalidException("User name is too short.") { InvalidValue = name };

        if (name.Length > 20)
            throw new UserCredentialsInvalidException("User name is too long.") { InvalidValue = name };

        if (await _sysDb.IsUserNameAssigned(name))
            throw new UserAlreadyExistsException("A user with this name already exists.");
    }

    private static void EnsureUserPasswordValid(string password)
    {
        if (password.Length < 4)
            throw new UserCredentialsInvalidException("Password is too short.") { InvalidValue = password };

        if (password.Length > 256)
            throw new UserCredentialsInvalidException("Password is too long.") { InvalidValue = password };
    }

    public async Task LogUserEvent(UserLogEvent logEvent)
    {
        await _sysDb.CreateLogEvent(logEvent);
        await _sysDb.SaveChanges();
    }

    public async Task LogLookupEvent(LookupLogEvent logEvent)
    {
        await _sysDb.CreateLogEvent(logEvent);
        await _sysDb.SaveChanges();
    }

    public async Task<LogEvent?> GetLogEvent(Guid logEventId)
        => await _sysDb.GetLogEvent(logEventId);

    public async Task<ICollection<LogEvent>> GetLogEvents(LogEventType? typeFilter = null, (DateTime start, DateTime end)? dateFilter = null)
    {
        var query = _sysDb.CreateLogQuery();

        if (dateFilter.HasValue)
            query = query.Where(e => e.Timestamp >= dateFilter.Value.start && e.Timestamp <= dateFilter.Value.end);

        if (typeFilter.HasValue)
        {
            if (typeFilter == LogEventType.Invalid)
                throw new ArgumentException("Type filter empty.", nameof(typeFilter));

            if ((typeFilter.Value & LogEventType.All) != typeFilter.Value)
                throw new ArgumentException("Type filter contains unknown flag(s).", nameof(typeFilter));

            var filter = typeFilter.Value & LogEventType.All;
            var allowedTypes = LogEventTypeToTypes(filter);

            return await _sysDb.AsAsyncLogQuery(query)
                .WhereAwait(e => ValueTask.FromResult(allowedTypes.Contains(e.GetType())))
                .ToArrayAsync();
        }

        return await _sysDb.ExecuteLogQuery(query);
    }
}
