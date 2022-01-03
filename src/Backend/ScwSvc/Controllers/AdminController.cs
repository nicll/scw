using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.Procedures.Interfaces;
using ScwSvc.SvcModels;
using static ScwSvc.Globals.Authorization;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers;

/// <summary>
/// Provides administrative features for admins and managers.
/// Managers generally only have read-only access to everything.
/// </summary>
[Route("api/[controller]")]
[Authorize(Policy = ManagerOrAdminOnly)]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthProcedures _authProc;
    private readonly IAdminProcedures _adminProc;

    public AdminController(ILogger<AdminController> logger, IMapper mapper, IAuthProcedures authProc, IAdminProcedures adminProc)
    {
        _logger = logger;
        _mapper = mapper;
        _authProc = authProc;
        _adminProc = adminProc;
    }

    [HttpGet("user")]
    public async Task<ICollection<User>> GetUsers()
        => await _adminProc.GetAllUsers();

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> GetUser([FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            var user = await _adminProc.GetUser(userId);

            if (user is null)
                return NotFound("User was not found.");

            return Ok(user);
        });

    [HttpPost("user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> AddUser([FromBody] AuthenticationCredentials credentials)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.AddUser(credentials.Username, credentials.Password);
                return Ok();
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest("A user with this name already exists.");
            }
            catch (UserCredentialsInvalidException e)
            {
                return BadRequest($"Credentials invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add user.");
                throw;
            }
        });

    [HttpDelete("user/{userId}")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteUser([FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.DeleteUser(userId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete user.");
                throw;
            }
        });

    [HttpPut("user/{userId}/username")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> ChangeUserName([FromRoute] Guid userId, [FromBody] string username)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.ChangeUserName(userId, username);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest("User with this name already exists.");
            }
            catch (UserModificationException e)
            {
                return BadRequest($"Change was invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not change user name.");
                throw;
            }
        });

    [HttpPut("user/{userId}/password")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> ChangeUserPassword([FromRoute] Guid userId, [FromBody] string password)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.ChangeUserPassword(userId, password);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
            catch (UserModificationException e)
            {
                return BadRequest($"Change was invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not change user password.");
                throw;
            }
        });

    [HttpPut("user/{userId}/role")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> ChangeUserRole([FromRoute] Guid userId, [FromBody] UserRole role)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.ChangeUserRole(userId, role);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not change user role.");
                throw;
            }
        });

    [HttpGet("user/{userId}/table")]
    [ProducesResponseType(typeof(ICollection<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> GetUserTables([FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.GetUserTables(userId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
        });

    [HttpGet("user/{userId}/table/own")]
    [ProducesResponseType(typeof(ICollection<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> GetUserTablesOwn([FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.GetUserTablesOwn(userId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
        });

    [HttpGet("user/{userId}/collaboration")]
    [ProducesResponseType(typeof(ICollection<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> GetUserCollaborations([FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.GetUserTablesCollaboration(userId);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("User was not found.");
            }
        });

    [HttpGet("table")]
    public async Task<ICollection<Table>> GetTables()
        => await _adminProc.GetAllTables();

    [HttpGet("dataset")]
    public async Task<ICollection<Table>> GetDataSets()
        => await _adminProc.GetAllDataSets();

    [HttpGet("sheet")]
    public async Task<ICollection<Table>> GetSheets()
        => await _adminProc.GetAllSheets();

    [HttpPost("dataset")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async ValueTask<IActionResult> CreateDataSet([FromBody] CreateDataSet dsModel)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                var table = _mapper.Map<Table>(dsModel);
                table = _adminProc.PrepareDataSet(user, table);
                await _adminProc.CreateDataSet(user, table);
                return Created("/api/data/dataset/" + table.TableId, table);
            }
            catch (TableAlreadyExistsException)
            {
                return Conflict("Try again.");
            }
            catch (TableDeclarationException e)
            {
                return BadRequest($"Table declaration invalid: {e.Message}");
            }
            catch (TableColumnException e)
            {
                return BadRequest($"Table column(s) invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not create data set.");
                throw;
            }
        });

    [HttpDelete("dataset/{tableId}")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> RemoveDataSet([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.DeleteDataSet(tableId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return NotFound("The data set could not be found in your tables.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("The table was not a data set.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete data set.");
                throw;
            }
        });

    [HttpPost("sheet")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async ValueTask<IActionResult> CreateSheet([FromBody] CreateSheet shModel)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                var table = _mapper.Map<Table>(shModel);
                table = _adminProc.PrepareSheet(user, table);
                await _adminProc.CreateSheet(user, table);
                return Created("/api/data/sheet/" + table.TableId, table);
            }
            catch (TableAlreadyExistsException)
            {
                return Conflict("Try again.");
            }
            catch (TableDeclarationException e)
            {
                return BadRequest($"Table declaration invalid: {e.Message}");
            }
            catch (TableColumnException e)
            {
                return BadRequest($"Table column(s) invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not create data set.");
                throw;
            }
        });

    [HttpDelete("sheet/{tableId}")]
    [Authorize(Policy = AdminOnly)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> RemoveSheet([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            try
            {
                await _adminProc.DeleteSheet(tableId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return NotFound("The sheet could not be found in your tables.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("The table was not a sheet.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete sheet.");
                throw;
            }
        });
}
