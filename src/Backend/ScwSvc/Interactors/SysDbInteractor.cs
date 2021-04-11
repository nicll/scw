using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Interactors
{
    public static class SysDbInteractor
    {
        /// <summary>
        /// Gets a <see cref="User"/> object by their <see cref="User.UserId"/>.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="id">The user's <see cref="User.UserId"/>.</param>
        /// <returns>The full <see cref="User"/> object.</returns>
        public static async ValueTask<User> GetUserById(this DbSysContext db, Guid id)
            => await db.Users.FindAsync(id).ConfigureAwait(false);

        /// <summary>
        /// Gets a <see cref="User"/> object by their <see cref="User.Name"/>.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="name">The user's <see cref="User.Name"/>.</param>
        /// <returns>The full <see cref="User"/> object.</returns>
        public static async ValueTask<User> GetUserByName(this DbSysContext db, string name)
            => await db.Users.FirstOrDefaultAsync(u => u.Name == name).ConfigureAwait(false);

        /// <summary>
        /// Gets all users of a specific <see cref="UserRole"/>.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="role">The user's <see cref="User.Role"/>.</param>
        /// <returns>A collection of all such users.</returns>
        public static async ValueTask<ICollection<User>> GetUsersByRole(this DbSysContext db, UserRole role)
            => await db.Users.Where(u => u.Role == role).ToArrayAsync().ConfigureAwait(false);

        /// <summary>
        /// Adds a new <see cref="User"/> object to the SYS database.
        /// </summary>
        /// <remarks>
        /// The new user must have a unique <see cref="User.UserId"/> and a unique <see cref="User.Name"/>.</remarks>
        /// <param name="db">The SYS database context.</param>
        /// <param name="user">The new <see cref="User"/> object.</param>
        public static async ValueTask AddUser(this DbSysContext db, User user, bool commit = true)
        {
            await db.Users.AddAsync(user).ConfigureAwait(false);

            if (commit)
                await db.SaveChangesAsync();
        }

        /// <summary>
        /// Checks whether a username is already in use.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="name">The user's <see cref="User.Name"/>.</param>
        /// <returns>Whether or not the username is already in use.</returns>
        public static async ValueTask<bool> IsUsernameAssigned(this DbSysContext db, string name)
            => await db.Users.AnyAsync(u => u.Name == name).ConfigureAwait(false);

        /// <summary>
        /// Gets a <see cref="TableRef"/> by its <see cref="TableRef.TableRefId"/>.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="id">The table's <see cref="TableRef.TableRefId"/>.</param>
        /// <returns>The full <see cref="TableRef"/> object.</returns>
        public static async ValueTask<TableRef> GetTableRefById(this DbSysContext db, Guid id)
            => await db.TableRefs.FindAsync(id).ConfigureAwait(false);

        /// <summary>
        /// Modifies a <see cref="User"/> object and optionally updates it in the SYS database.
        /// </summary>
        /// <remarks>
        /// This method performs additional checking for validity.
        /// If any validation fails then a <see cref="UserChangeException"/> is thrown.
        /// </remarks>
        /// <param name="db">The SYS database context.</param>
        /// <param name="user">The unmodified <see cref="User"/> object.</param>
        /// <param name="commit">Whether or not to save changes to the database.</param>
        /// <param name="username">The new username.</param>
        /// <param name="password">The new password.</param>
        /// <exception cref="UserChangeException">If any validation fails.</exception>
        public static async ValueTask ModifyUser(this DbSysContext db, User user, bool commit = true, string username = null, string password = null)
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
                if (String.IsNullOrEmpty(password) || password.Length < 4) // ToDo: change to more sensible value when testing is finished
                    throw new UserChangeException("Passwort empty or too short.") { UserId = user.UserId, OldValue = "(old password)", NewValue = "(new password)" };

                user.PasswordHash = HashUserPassword(user.UserId, password);
            }

            if (commit)
                await db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a <see cref="User"/> object and optionally updates the SYS database.
        /// Also removes all references to this <see cref="User"/> object.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="user">The <see cref="User"/> object to remove.</param>
        /// <param name="commit">Whether or not to save changes to the database.</param>
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

        /// <summary>
        /// Removes a <see cref="TableRef"/> object and optionally updates the SYS database.
        /// Also removes all references to this <see cref="TableRef"/> object.
        /// </summary>
        /// <param name="db">The SYS database context.</param>
        /// <param name="table">The <see cref="TableRef"/> object to remove.</param>
        /// <param name="commit">Whether or not to save changes to the database.</param>
        public static async ValueTask RemoveTable(this DbSysContext db, TableRef table, bool commit = false)
        {
            foreach (var user in await db.Users.ToArrayAsync().ConfigureAwait(false))
                user.Collaborations.Remove(table);
            table.Owner.OwnTables.Remove(table);
            db.TableRefs.Remove(table);

            if (commit)
                await db.SaveChangesAsync();
        }
    }
}
