using ScwSvc.Models;

namespace ScwSvc.Operations.Interfaces;

public interface IUserOperations
{
    Task<ICollection<User>> GetUsers();

    Task<User?> GetUserById(Guid id);

    Task<User?> GetUserByName(string name);

    Task ModifyUser(Guid id, string? name, string? password, UserRole? role);

    Task<Guid> AddUser(string name, string password);

    Task DeleteUser(Guid id);

    Task<User?> LoginUser(string name, string password);
}
