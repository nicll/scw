using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScwSvc.Models;

namespace ScwSvc.BusinessLogic.Interfaces;

public interface IUserLogic
{
    Task<IEnumerable<User>> GetUsers();

    Task<User> GetUserById(Guid userId);

    Task AddUser(User user);

    Task RemoveUser(User user);

    Task ChangeUserName(User user, string name);

    Task ChangeUserRole(User user, UserRole role);

    Task ChangeUserPassword(User user, string password);
}
