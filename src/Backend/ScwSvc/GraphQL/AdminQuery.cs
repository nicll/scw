using System.Linq;
using HotChocolate;
using ScwSvc.DataAccess.Interfaces;
using ScwSvc.Models;

namespace ScwSvc.GraphQL;

public class AdminQuery
{
    public IQueryable<User> GetUsers([Service] ISysDbRepository sysDb)
        => sysDb.CreateUsersQuery();

    public IQueryable<Table> GetTables([Service] ISysDbRepository sysDb)
        => sysDb.CreateTablesQuery();
}
