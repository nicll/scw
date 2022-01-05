using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class ServiceProcedures : IServiceProcedures
{
    private readonly IUserOperations _userOp;

    public ServiceProcedures(IUserOperations userOp)
        => _userOp = userOp;

    public async Task<Guid> RegisterUser(string name, string password)
        => await _userOp.AddUser(name, password);

    public async Task<User?> LoginUser(string username, string password)
    {
        var user = await _userOp.LoginUser(username, password);

        if (user is not null)
            ; // log login

        return user;
    }
}
