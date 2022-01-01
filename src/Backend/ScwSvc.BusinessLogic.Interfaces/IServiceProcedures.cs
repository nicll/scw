using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IServiceProcedures
{
    Task<Guid> RegisterUser(string name, string password);

    Task<User?> LoginUser(string username, string password);
}
