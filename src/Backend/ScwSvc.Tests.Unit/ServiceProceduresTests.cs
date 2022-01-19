using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ScwSvc.Models;
using ScwSvc.Operations.Interfaces;
using ScwSvc.Procedures.Impl;

namespace ScwSvc.Tests.Unit;

public class ServiceProceduresTests
{
    private ServiceProcedures _serviceProc;
    private int _AddUserCalls, _LoginUserCalls, _LogUserEventCalls;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        var mock = new Mock<IUserOperations>();
        mock.Setup(u => u.AddUser(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() =>
            {
                ++_AddUserCalls;
                return Task.FromResult(Guid.Empty);
            });
        mock.Setup(u => u.LoginUser(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() =>
            {
                ++_LoginUserCalls;
                return Task.FromResult(new User());
            });
        mock.Setup(u => u.LogUserEvent(It.IsAny<UserLogEvent>()))
            .Returns(() =>
            {
                ++_LogUserEventCalls;
                return Task.CompletedTask;
            });
        mock.Setup(u => u.GetUserById(It.IsAny<Guid>()))
            .Returns(() => Task.FromResult(new User()));

        _serviceProc = new(mock.Object);
    }

    [SetUp]
    public void Setup()
    {
        _AddUserCalls = 0;
        _LoginUserCalls = 0;
        _LogUserEventCalls = 0;
    }

    [Test]
    public async Task RegisterUser_CallsAddUserOnce()
    {
        await _serviceProc.RegisterUser("name", "pass");
        Assert.AreEqual(1, _AddUserCalls);
        Assert.AreEqual(1, _LogUserEventCalls);
    }

    [Test]
    public async Task LoginUser_CallsLoginUserOnce()
    {
        await _serviceProc.LoginUser("name", "pass");
        Assert.AreEqual(1, _LoginUserCalls);
        Assert.AreEqual(1, _LogUserEventCalls);
    }

    [Test]
    public async Task LogoutUser_CallsLogUserEventOnce()
    {
        await _serviceProc.LogoutUser(Guid.Empty);
        Assert.AreEqual(1, _LogUserEventCalls);
    }
}
