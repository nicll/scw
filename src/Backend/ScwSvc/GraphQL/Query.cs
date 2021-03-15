using HotChocolate;
using ScwSvc.Models;
using System;
using System.Linq;

namespace ScwSvc.GraphQL
{
    /// <summary>
    /// Contains all APIs made available with GraphQL.
    /// Currently just for experimentation.
    /// </summary>
    public class Query
    {
#if DEBUG // just for trying out
        public IQueryable<User> GetUsers([Service] DbSysContext sysDb)
            => sysDb.Users;

        public IQueryable<TableRef> GetTableRefs([Service] DbSysContext sysDb)
            => sysDb.TableRefs;
#endif
    }
}
