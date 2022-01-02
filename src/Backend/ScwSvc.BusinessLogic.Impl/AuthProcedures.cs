using System.Threading.Tasks;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Impl;

public class AuthProcedures
{
    private readonly IUserOperations _user;

    public AuthProcedures(IUserOperations user)
        => _user = user;

    public async Task<User?> GetUserById(Guid id)
        => await _user.GetUserById(id);
}
