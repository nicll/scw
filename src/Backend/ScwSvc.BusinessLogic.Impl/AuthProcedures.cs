﻿using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class AuthProcedures : IAuthProcedures
{
    private readonly IUserOperations _user;

    public AuthProcedures(IUserOperations user)
        => _user = user;

    public async Task<User?> GetUserById(Guid id)
        => await _user.GetUserById(id);
}
