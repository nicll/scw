using Moq;
using NUnit.Framework;
using ScwSvc.Operations.Interfaces;
using ScwSvc.Procedures.Impl;

namespace ScwSvc.Tests.Unit;

public class AdminProceduresTests
{
    private AdminProcedures _adminProc;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        var userOp = new Mock<IUserOperations>().Object;
        var tableOp = new Mock<ITableOperations>().Object;
        _adminProc = new(userOp, tableOp);
    }
}
