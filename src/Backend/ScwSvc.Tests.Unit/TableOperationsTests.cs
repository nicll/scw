using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.DataAccess.Impl;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.Operations.Impl;

namespace ScwSvc.Tests.Unit;

public class TableOperationsTests
{
    private DbSysContext _sysDb;
    private DbDynContext _dynDb;
    private TableOperations _tableOp;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        _sysDb = new DbSysContext(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase(nameof(TableOperationsTests)).Options);
        _dynDb = new DbDynContext(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase(nameof(TableOperationsTests)).Options);

        _tableOp = new TableOperations(
            new SysDbRepository(_sysDb),
            new DynDbRepository(_dynDb));
    }

    [Test, Order(1)]
    public void AddTable_InvalidTableType_TableMismatchException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "asdf",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            TableType = (TableType)(-1)
        };

        Assert.ThrowsAsync<TableMismatchException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(2)]
    public void AddTable_InvalidTableId_TableDeclarationException()
    {
        var table = new Table
        {
            TableId = Guid.Empty,
            DisplayName = "asdf",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            TableType = (TableType)1
        };

        Assert.ThrowsAsync<TableDeclarationException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(3)]
    public void AddTable_InvalidLookupName_TableDeclarationException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "asdf",
            LookupName = Guid.Empty,
            OwnerUserId = Guid.NewGuid(),
            TableType = (TableType)1
        };

        Assert.ThrowsAsync<TableDeclarationException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(4)]
    public void AddTable_InvalidOwnerId_TableDeclarationException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "asdf",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.Empty,
            TableType = (TableType)1
        };

        Assert.ThrowsAsync<TableDeclarationException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(5)]
    public void AddTable_InvalidDisplayName_TableDeclarationException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            TableType = (TableType)1
        };

        Assert.ThrowsAsync<TableDeclarationException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(6)]
    public void AddTable_SheetColumns_TableColumnException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "asdf",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            TableType = TableType.Sheet,
            Columns = new DataSetColumn[1]
        };

        Assert.ThrowsAsync<TableColumnException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(7)]
    public void AddTable_DataSetNoColumns_TableColumnException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "asdf",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            TableType = TableType.DataSet,
            Columns = new DataSetColumn[0]
        };

        Assert.ThrowsAsync<TableColumnException>(async () => await _tableOp.AddTable(table));
    }

    [Test, Order(8)]
    public void AddTable_DataSetManyColumns_TableColumnException()
    {
        var table = new Table
        {
            TableId = Guid.NewGuid(),
            DisplayName = "asdf",
            LookupName = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            TableType = TableType.DataSet,
            Columns = new DataSetColumn[_tableOp.MaxNumberOfColumns + 1]
        };

        Assert.ThrowsAsync<TableColumnException>(async () => await _tableOp.AddTable(table));
    }
}
