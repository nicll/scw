using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Interactors
{
    public static class SysDbInteractor
    {
        public static async ValueTask<User> GetUserById(this DbSysContext db, Guid id)
            => await db.Users.FindAsync(id).ConfigureAwait(false);

        public static async ValueTask<User> GetUserByName(this DbSysContext db, string name)
            => await db.Users.FirstOrDefaultAsync(u => u.Name == name).ConfigureAwait(false);

        public static async ValueTask AddUser(this DbSysContext db, User user)
            => await db.Users.AddAsync(user).ConfigureAwait(false);

        public static async ValueTask<bool> IsUsernameAssigned(this DbSysContext db, string name)
            => await db.Users.AnyAsync(u => u.Name == name).ConfigureAwait(false);

        public static async ValueTask<TableRef> GetTableRefById(this DbSysContext db, Guid id)
            => await db.TableRefs.FindAsync(id).ConfigureAwait(false);

        public static async ValueTask RemoveUser(this DbSysContext db, User user, bool commit = false)
        {
            await db.TableRefs
                .Where(t => t.Collaborators.Contains(user))
                .ForEachAsync(t => t.Collaborators.Remove(user))
                .ConfigureAwait(false);
            db.TableRefs.RemoveRange(user.OwnTables);
            db.Users.Remove(user);

            if (commit)
                await db.SaveChangesAsync();
        }

        public static async ValueTask RemoveTable(this DbSysContext db, TableRef table, bool commit = false)
        {
            await db.Users.ForEachAsync(u => u.Collaborations.Remove(table)).ConfigureAwait(false);
            table.Owner.OwnTables.Remove(table);
            db.TableRefs.Remove(table);

            if (commit)
                await db.SaveChangesAsync();
        }
    }
}
