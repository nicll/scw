using System.Linq;
using HotChocolate;
using ScwSvc.Models;
using ScwSvc.SvcModels;

namespace ScwSvc.GraphQL
{
    public class AdminQuery
    {
        public IQueryable<User> GetUsers([Service] DbSysContext sysDb)
            => sysDb.Users;

        public IQueryable<TableRef> GetTableRefs([Service] DbSysContext sysDb)
            => sysDb.TableRefs;
    }
}
