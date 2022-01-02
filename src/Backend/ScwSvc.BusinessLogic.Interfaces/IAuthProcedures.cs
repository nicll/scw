using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IAuthProcedures
{
    Task<User?> GetUserById(Guid id);
}
