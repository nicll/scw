using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.Repositories;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Tests.Unit
{
    public class SysDbRepositoryTests
    {
        private const string
            CommonUserId = "00000000-0000-0000-0000-000000000001",
            ManagerUserId = "00000000-0000-0000-0000-000000000002",
            AdminUserId = "00000000-0000-0000-0000-000000000003";
        private DbSysContext _sysDb;
        private readonly User
            _commonUser =  new() { Name = "CommonUser",  Role = UserRole.Common,  UserId = Guid.Parse(CommonUserId), PasswordHash = new byte[32], Collaborations = new List<TableRef>() },
            _managerUser = new() { Name = "ManagerUser", Role = UserRole.Manager, UserId = Guid.Parse(ManagerUserId), Collaborations = new List<TableRef>(), OwnTables = Array.Empty<TableRef>() },
            _adminUser =   new() { Name = "AdminUser",   Role = UserRole.Admin,   UserId = Guid.Parse(AdminUserId), Collaborations = new List<TableRef>(), OwnTables = Array.Empty<TableRef>() };
        private readonly TableRef
            _datasetTable = new() { TableRefId = Guid.NewGuid(), TableType = TableType.DataSet, OwnerUserId = Guid.Parse(CommonUserId) },
            _sheetTable =   new() { TableRefId = Guid.NewGuid(), TableType = TableType.Sheet, OwnerUserId = Guid.Parse(CommonUserId) };

        [OneTimeSetUp]
        public void SetupOnce()
        {
            _sysDb = new DbSysContext(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase("test").UseLazyLoadingProxies().Options);
            //_commonUser.OwnTables = new[] { _datasetTable, _sheetTable };
            _datasetTable.Owner = _commonUser;
            _sheetTable.Owner = _commonUser;
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

        [Test, Order(8)]
        public async Task ModifyUserUsername()
        {
            throw new InconclusiveException("Known defect: see ToDo in DbSysContext.cs");

            await _sysDb.ModifyUser(_commonUser, commit: true, username: nameof(_commonUser));

            var foundUser = await _sysDb.GetUserByName(_commonUser.Name);

            Assert.AreEqual(nameof(_commonUser), _commonUser.Name);
            Assert.AreEqual(_commonUser, foundUser);
        }

        [Test, Order(9)]
        public async Task ModifyUserPassword()
        {
            var oldPasswordHash = _commonUser.PasswordHash;

            await _sysDb.ModifyUser(_commonUser, commit: true, password: nameof(_commonUser));

            var foundUser = await _sysDb.GetUserByName(_commonUser.Name);

            Assert.IsFalse(oldPasswordHash.SequenceEqual(_commonUser.PasswordHash));
            Assert.IsFalse(oldPasswordHash.SequenceEqual(foundUser.PasswordHash));
        }

        [Test, Order(10)]
        public async Task RemoveUser()
        {
            await _sysDb.RemoveUser(_managerUser, commit: true);

            Assert.IsNull(await _sysDb.GetUserById(_managerUser.UserId));
            Assert.IsNull(await _sysDb.GetUserByName(_managerUser.Name));
            Assert.IsEmpty(await _sysDb.GetUsersByRole(_managerUser.Role));
        }

        [Test, Order(11)]
        public async Task CreateDataSet()
        {
            var tableRef = _datasetTable;

            await _sysDb.AddTable(tableRef, commit: true);

            Assert.AreEqual(tableRef, await _sysDb.GetTableRefById(tableRef.TableRefId));
        }

        [Test, Order(12)]
        public async Task CreateSheet()
        {
            var tableRef = _sheetTable;

            await _sysDb.AddTable(tableRef, commit: true);

            Assert.AreEqual(tableRef, await _sysDb.GetTableRefById(tableRef.TableRefId));
        }

        [Test, Order(13)]
        public async Task DeleteDataSet()
        {
            var tableRef = _datasetTable;

            await _sysDb.RemoveTable(tableRef, commit: true);

            Assert.IsNull(await _sysDb.GetTableRefById(tableRef.TableRefId));
        }

        [Test, Order(14)]
        public async Task DeleteSheet()
        {
            var tableRef = _sheetTable;

            await _sysDb.RemoveTable(tableRef, commit: true);

            Assert.IsNull(await _sysDb.GetTableRefById(tableRef.TableRefId));
        }
    }
}
