using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.DataAccess.Impl;
using ScwSvc.Operations.Impl;
using ScwSvc.Operations.Interfaces;

namespace ScwSvc.Tests.Unit;

public class LogOperationsTests
{
    private ITableOperations _tableOp;
    private IUserOperations _userOp;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        _tableOp = new TableOperations(
            new SysDbRepository(new(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase(nameof(LogOperationsTests)).Options)),
            new DynDbRepository(new(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase(nameof(LogOperationsTests)).Options)));

        _userOp = new UserOperations(
            new SysDbRepository(new(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase(nameof(LogOperationsTests)).Options)),
            new DynDbRepository(new(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase(nameof(LogOperationsTests)).Options)));
    }

    [Test, Order(1)]
    public async Task Test()
    {

    }
}
