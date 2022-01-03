using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class ServiceProcedures : IServiceProcedures
{
    private readonly IUserOperations _user;

    public ServiceProcedures(IUserOperations user)
        => _user = user;

    public async Task<Guid> RegisterUser(string name, string password)
        => await _user.AddUser(name, password);

    public async Task<User?> LoginUser(string username, string password)
    {
        var user = await _user.LoginUser(username, password);

        if (user is not null)
            ; // log login

        return user;
    }
}
