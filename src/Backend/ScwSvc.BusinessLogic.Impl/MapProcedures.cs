using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class MapProcedures : IMapProcedures
{
    private readonly IUserOperations _userOp;

    public MapProcedures(IUserOperations userOp)
        => _userOp = userOp;

    public async Task<string?> UserIdToName(Guid id)
    {
        var user = await _userOp.GetUserById(id);
        return user?.Name;
    }

    public async Task<Guid?> UserNameToId(string name)
    {
        var user = await _userOp.GetUserByName(name);
        return user?.UserId;
    }
}
