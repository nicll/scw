using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.DataAccess.Impl;
using ScwSvc.Models;
using ScwSvc.Operations.Impl;

namespace ScwSvc.Tests.Unit;

public class LogOperationsTests
{
    private TableOperations _tableOp;
    private UserOperations _userOp;

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
    public void LogUserLogEvent_ValidEntry_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () =>
            await _userOp.LogUserEvent(new UserLogEvent() { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, UserAction = UserLogEventType.Login })
        );
    }

    [Test, Order(2)]
    public void LogLookupLogEvent_ValidEntry_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () =>
            await _userOp.LogLookupEvent(new LookupLogEvent() { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow })
        );
    }

    [Test, Order(3)]
    public void LogTableDefinitionLogEvent_ValidEntry_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () =>
            await _tableOp.LogTableEvent(new TableDefinitionLogEvent() { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, TableAction = TableDefinitionLogEventType.CreateTable })
        );
    }

    [Test, Order(4)]
    public void LogTableCollaboratorLogEvent_ValidEntry_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () =>
            await _tableOp.LogTableEvent(new TableCollaboratorLogEvent() { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, CollaboratorAction = TableCollaboratorLogEventType.AddCollaborator })
        );
    }

    [Test, Order(5)]
    public async Task GetLogEvents_Unfiltered_Returns4Events()
    {
        var logEvents = await _userOp.GetLogEvents();

        Assert.AreEqual(4, logEvents.Count);
    }

    [Test, Order(6)]
    public async Task GetLogEvents_UserActions_Returns1Event()
    {
        var logEvents = await _userOp.GetLogEvents(typeFilter: LogEventType.User);

        Assert.AreEqual(1, logEvents.Count);
    }

    [Test, Order(7)]
    public async Task GetLogEvents_LookupActions_Returns1Event()
    {
        var logEvents = await _userOp.GetLogEvents(typeFilter: LogEventType.Lookup);

        Assert.AreEqual(1, logEvents.Count);
    }

    [Test, Order(8)]
    public async Task GetLogEvents_TableActions_Returns2Events()
    {
        var logEvents = await _userOp.GetLogEvents(typeFilter: LogEventType.Table);

        Assert.AreEqual(2, logEvents.Count);
    }

    [Test, Order(9)]
    public async Task GetLogEvents_TableDefinitionActions_Returns1Event()
    {
        var logEvents = await _userOp.GetLogEvents(typeFilter: LogEventType.TableDefinition);

        Assert.AreEqual(1, logEvents.Count);
    }

    [Test, Order(10)]
    public async Task GetLogEvents_TableActions_Returns1Event()
    {
        var logEvents = await _userOp.GetLogEvents(typeFilter: LogEventType.TableCollaboration);

        Assert.AreEqual(1, logEvents.Count);
    }

    [Test, Order(11)]
    public async Task GetLogEvents_DateOutOfRange_Returns0Events()
    {
        var logEvents = await _userOp.GetLogEvents(dateFilter: (DateTime.UnixEpoch, DateTime.UnixEpoch));

        Assert.AreEqual(0, logEvents.Count);
    }

    [Test, Order(12)]
    public async Task GetLogEvents_CatchAllFilters_Returns4Events()
    {
        var logEvents = await _userOp.GetLogEvents(typeFilter: LogEventType.All, dateFilter: (DateTime.MinValue, DateTime.MaxValue));

        Assert.AreEqual(4, logEvents.Count);
    }
}
