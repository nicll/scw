using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class MapProcedures : IMapProcedures
{
    private readonly IUserOperations _user;

    public MapProcedures(IUserOperations user)
        => _user = user;

    public async Task<string?> UserIdToName(Guid id)
    {
        var user = await _user.GetUserById(id);
        return user?.Name;
    }

    public async Task<Guid?> UserNameToId(string name)
    {
        var user = await _user.GetUserByName(name);
        return user?.UserId;
    }
}
