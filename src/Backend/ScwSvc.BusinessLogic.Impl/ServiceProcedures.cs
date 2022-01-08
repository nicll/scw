using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class ServiceProcedures : IServiceProcedures
{
    private readonly IUserOperations _userOp;

    public ServiceProcedures(IUserOperations userOp)
        => _userOp = userOp;

    public async Task<Guid> RegisterUser(string name, string password)
    {
        var userId = await _userOp.AddUser(name, password);
        await _userOp._LogUserEvent(userId, UserLogEventType.CreateUser);
        return userId;
    }

    public async Task<User?> LoginUser(string username, string password)
    {
        var user = await _userOp.LoginUser(username, password);

        if (user is not null)
            await _userOp._LogUserEvent(user.UserId, UserLogEventType.Login);

        return user;
    }

    public async Task LogoutUser(Guid userId)
    {
        var user = await _userOp.GetUserById(userId);

        if (user is not null)
            await _userOp._LogUserEvent(user.UserId, UserLogEventType.Logout);
    }
}
