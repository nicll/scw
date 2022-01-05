using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class AuthProcedures : IAuthProcedures
{
    private readonly IUserOperations _userOp;

    public AuthProcedures(IUserOperations userOp)
        => _userOp = userOp;

    public async Task<User?> GetUserById(Guid id)
        => await _userOp.GetUserById(id);
}
