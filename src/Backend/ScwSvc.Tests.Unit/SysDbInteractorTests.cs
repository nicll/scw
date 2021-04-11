using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.Interactors;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Tests.Unit
{
    public class SysDbInteractorTests
    {
        private DbSysContext _sysDb;
        private readonly User
            _commonUser =  new() { Name = "CommonUser",  Role = UserRole.Common,  UserId = Guid.NewGuid() },
            _managerUser = new() { Name = "ManagerUser", Role = UserRole.Manager, UserId = Guid.NewGuid() },
            _adminUser =   new() { Name = "AdminUser",   Role = UserRole.Admin,   UserId = Guid.NewGuid() };

        [OneTimeSetUp]
        public void SetupOnce()
        {
            _sysDb = new DbSysContext(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase("test").UseLazyLoadingProxies().Options);
        }

        [Test, Order(1)]
        public async Task CreateCommonUser()
        {
            var user = _commonUser;

            await _sysDb.AddUser(user);
            await _sysDb.SaveChangesAsync();

            Assert.Contains(_commonUser, await _sysDb.Users.ToArrayAsync());
        }

        [Test, Order(2)]
        public async Task CreateManagerUser()
        {
            var user = _managerUser;

            await _sysDb.AddUser(user);

            Assert.Contains(_managerUser, await _sysDb.Users.ToArrayAsync());
        }

        [Test, Order(3)]
        public async Task CreateAdminUser()
        {
            var user = _adminUser;

            await _sysDb.AddUser(user);

            Assert.Contains(_adminUser, await _sysDb.Users.ToArrayAsync());
        }

        [Test, Order(4)]
        public async Task GetUserById()
        {
            var foundUser = await _sysDb.GetUserById(_commonUser.UserId);

            Assert.AreEqual(_commonUser, foundUser);
        }

        [Test, Order(5)]
        public async Task GetUserByName()
        {
            var foundUser = await _sysDb.GetUserByName(_managerUser.Name);

            Assert.AreEqual(_managerUser, foundUser);
        }

        [Test, Order(6)]
        public async Task GetUsersByRole()
        {
            var foundUsers = await _sysDb.GetUsersByRole(_adminUser.Role);

            Assert.AreEqual(_adminUser, foundUsers.Single());
        }

        [Test, Order(7)]
        public async Task AreUsernamesAssigned()
        {
            Assert.IsTrue(await _sysDb.IsUsernameAssigned(_commonUser.Name));
            Assert.IsTrue(await _sysDb.IsUsernameAssigned(_managerUser.Name));
            Assert.IsTrue(await _sysDb.IsUsernameAssigned(_adminUser.Name));
        }
    }
}
