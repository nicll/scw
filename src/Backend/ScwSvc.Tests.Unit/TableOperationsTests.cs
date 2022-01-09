using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.DataAccess.Impl;
using ScwSvc.Operations.Impl;
using ScwSvc.Operations.Interfaces;

namespace ScwSvc.Tests.Unit;

public class TableOperationsTests
{
    private ITableOperations _tableOp;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        _tableOp = new TableOperations(
            new SysDbRepository(new(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase(nameof(TableOperationsTests)).Options)),
            new DynDbRepository(new(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase(nameof(TableOperationsTests)).Options)));
    }

    [Test, Order(1)]
    public async Task Test()
    {

    }
}
