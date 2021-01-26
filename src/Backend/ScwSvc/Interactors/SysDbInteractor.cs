using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Utils.Authentication;

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

        public static async ValueTask ModifyUser(this DbSysContext db, User user, bool commit = false, string username = null, string password = null)
        {
            if (username is not null)
            {
                if (String.IsNullOrEmpty(username) || username.Length > 20)
                    throw new UserChangeException("Invalid username given.") { UserId = user.UserId, OldValue = user.Name, NewValue = username };

                if (await IsUsernameAssigned(db, username))
                    throw new UserChangeException("Username is already in use.") { UserId = user.UserId, OldValue = user.Name, NewValue = username };

                user.Name = username;
            }

            if (password is not null)
            {
                if (String.IsNullOrEmpty(username) || username.Length < 4) // ToDo: change to more sensible value when testing is finished
                    throw new UserChangeException("Passwort empty or too short.") { UserId = user.UserId, OldValue = user.Name, NewValue = username };

                user.PasswordHash = HashUserPassword(user.UserId, password);
            }

            if (commit)
                await db.SaveChangesAsync().ConfigureAwait(false);
        }

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
