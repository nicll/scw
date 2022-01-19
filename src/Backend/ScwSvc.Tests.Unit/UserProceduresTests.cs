using Moq;
using NUnit.Framework;
using ScwSvc.Operations.Interfaces;
using ScwSvc.Procedures.Impl;

namespace ScwSvc.Tests.Unit;

public class UserProceduresTests
{
    private UserProcedures _userProc;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        var userOp = new Mock<IUserOperations>().Object;
        var tableOp = new Mock<ITableOperations>().Object;
        _userProc = new(userOp, tableOp);
    }
}
