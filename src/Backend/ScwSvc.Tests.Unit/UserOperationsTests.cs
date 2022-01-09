using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScwSvc.DataAccess.Impl;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.Operations.Impl;
using ScwSvc.Operations.Interfaces;

namespace ScwSvc.Tests.Unit;

public class UserOperationsTests
{
    private IUserOperations _userOp;
    private Guid _userId;

    [OneTimeSetUp]
    public void SetupOnce()
    {
        _userOp = new UserOperations(
            new SysDbRepository(new(new DbContextOptionsBuilder<DbSysContext>().UseInMemoryDatabase(nameof(UserOperationsTests)).Options)),
            new DynDbRepository(new(new DbContextOptionsBuilder<DbDynContext>().UseInMemoryDatabase(nameof(UserOperationsTests)).Options)));
    }

    [Test, Order(1)]
    public async Task AddAndCheckUser_Valid_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () => _userId = await _userOp.AddUser("first", "first"));
    }

    [Test, Order(2)]
    public void AddUser_Duplicate_UserAlreadyExists()
    {
        Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _userOp.AddUser("first", "first"));
    }

    [Test, Order(3)]
    public async Task GetUserById_Valid_NotNull()
    {
        var user = await _userOp.GetUserById(_userId);

        Assert.NotNull(user);
        Assert.AreEqual("first", user.Name);
        Assert.AreEqual(UserRole.Common, user.Role);
    }

    [Test, Order(4)]
    public async Task GetUserByName_Valid_NotNull()
    {
        var user = await _userOp.GetUserByName("first");

        Assert.NotNull(user);
        Assert.AreEqual("first", user.Name);
        Assert.AreEqual(UserRole.Common, user.Role);
    }

    [Test, Order(5)]
    public async Task LoginUser_Valid_NotNull()
    {
        var user = await _userOp.LoginUser("first", "first");

        Assert.NotNull(user);
    }

    [Test, Order(6)]
    public async Task LoginUser_WrongPassword_Null()
    {
        var user = await _userOp.LoginUser("first", "second");

        Assert.Null(user);
    }

    [Test, Order(7)]
    public void LoginUser_BadCredentials_UserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _userOp.LoginUser("second", "first"));
    }

    [Test, Order(8)]
    public async Task ModifyUser_ValidChanges_NoThrow()
    {
        await _userOp.ModifyUser(_userId, "newfirst", "newfirst", UserRole.Admin);
    }

    [Test, Order(9)]
    public async Task LoginUser_ValidAfterChange_NotNull()
    {
        var user = await _userOp.LoginUser("newfirst", "newfirst");

        Assert.NotNull(user);
    }

    [Test, Order(10)]
    public void AddUser_LongName_Exception()
    {
        Assert.ThrowsAsync<UserCredentialsInvalidException>(async () => await _userOp.AddUser("XXXXXXXXXXXXXXXXXXXXYYYY", "XY"));
    }

    [Test, Order(11)]
    public void AddUser_ValidCredentials_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () => await _userOp.AddUser("second", "second"));
    }

    [Test, Order(12)]
    public void ModifyUser_DuplicateName_UserAlreadyExistsException()
    {
        Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _userOp.ModifyUser(_userId, "second", null, null));
    }

    [Test, Order(13)]
    public void ModifyUser_LongName_UserModificationException()
    {
        Assert.ThrowsAsync<UserModificationException>(async () => await _userOp.ModifyUser(_userId, "XXXXXXXXXXXXXXXXXXXXYYYY", null, null));
    }

    [Test, Order(14)]
    public async Task GetUsers_Returns2Users()
    {
        var users = await _userOp.GetUsers();

        Assert.AreEqual(2, users.Count);
    }

    [Test, Order(15)]
    public void DeleteUser_InvalidId_UserNotFoundException()
    {
        Assert.ThrowsAsync<UserNotFoundException>(async () => await _userOp.DeleteUser(Guid.Empty));
    }

    [Test, Order(16)]
    public void DeleteUser_ValidId_NoThrow()
    {
        Assert.DoesNotThrowAsync(async () => await _userOp.DeleteUser(_userId));
    }

    [Test, Order(17)]
    public async Task GetUsers_Returns1User()
    {
        var users = await _userOp.GetUsers();

        Assert.AreEqual(1, users.Count);
    }
}
