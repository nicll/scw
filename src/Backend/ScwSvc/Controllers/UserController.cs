using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Procedures.Interfaces;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.SvcModels;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers;

[Route("api/my")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthProcedures _authProc;
    private readonly IUserProcedures _userProc;

    public UserController(ILogger<UserController> logger, IMapper mapper, IAuthProcedures authProc, IUserProcedures userProc)
    {
        _logger = logger;
        _mapper = mapper;
        _authProc = authProc;
        _userProc = userProc;
    }

    [HttpGet("username")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyUsername()
        => await AuthenticateAndRun(_authProc, User, user => Ok(user.Name));

    [HttpPatch("username")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> ChangeUsername([FromBody] string username)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.ChangeUserName(user, username);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("This user does not exist.");
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest("A user with this name already exists.");
            }
            catch (UserModificationException e)
            {
                return BadRequest($"The change was invalid: {e.OldValue} -> {e.NewValue}; {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while changing user name.");
                throw;
            }
        });

    [HttpPatch("password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> ChangePassword([FromBody] string password)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.ChangeUserPassword(user, password);
                return Ok();
            }
            catch (UserNotFoundException)
            {
                return NotFound("This user does not exist.");
            }
            catch (UserModificationException e)
            {
                return BadRequest($"The change was invalid: {e.OldValue} -> {e.NewValue}; {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while changing user password.");
                throw;
            }
        });

    /// <summary>
    /// Queries a collection of all data sets that the user may access.
    /// </summary>
    /// <returns>A collection of all accessible data sets.</returns>
    [HttpGet("dataset")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsAll()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries a collection of all data sets that the user owns.
    /// </summary>
    /// <returns>A collection of all of the user's data sets.</returns>
    [HttpGet("dataset/own")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsOwn()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Where(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries the number of all data sets that the user owns.
    /// </summary>
    /// <returns>The number of all of the user's data sets.</returns>
    [HttpGet("dataset/own/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsOwnCount()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Count(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries the remaining number of data sets that the user can create.
    /// </summary>
    /// <returns>The remaining number of the user's data sets.</returns>
    [HttpGet("dataset/own/remaining")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsOwnRemaining()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(_userProc.MaxDataSetsPerUser - user.OwnTables.Count(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries a collection of other people's data sets that the user may access.
    /// </summary>
    /// <returns>A collection of all of the data sets shared with the user.</returns>
    [HttpGet("dataset/collaborations")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsCollaborations()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.Collaborations.Where(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries a single data set that a user may access.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <returns>The table reference of the data set.</returns>
    [HttpGet("dataset/{tableId}")]
    [ProducesResponseType(typeof(Table), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> MyDataSet([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                return Ok(await _userProc.GetDataSet(user, tableId));
            }
            catch (TableNotFoundException)
            {
                return NotFound("No table with this ID was found.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("Found table of different type.");
            }
        });

    /// <summary>
    /// Creates a new data set for a user.
    /// </summary>
    /// <param name="dsModel">The definition of the data set.</param>
    /// <returns>The table reference for the new data set.</returns>
    [HttpPost("dataset")]
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
                table = _userProc.PrepareDataSet(user, table);
                await _userProc.CreateDataSet(user, table);
                return Created("/api/data/dataset/" + table.TableId, table);
            }
            catch (TableLimitExceededException)
            {
                return BadRequest("You cannot create any more additional data sets.");
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

    /// <summary>
    /// Deletes the data set of a user.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    [HttpDelete("dataset/{tableId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteDataSet([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.DeleteDataSet(user, tableId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return BadRequest("The data set could not be found in your tables.");
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

    /// <summary>
    /// Adds a column to an existing table.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <param name="columnName">The name of the new column.</param>
    /// <param name="columnDef">The definition of the new column.</param>
    [HttpPost("dataset/{tableId}/{columnName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> AddColumnToDataSet([FromRoute] Guid tableId, [FromRoute] string columnName, [FromBody] ColumnDefinition columnDef)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                var column = _mapper.Map<DataSetColumn>(columnDef);
                await _userProc.AddDataSetColumn(user, tableId, column);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return NotFound("The table was not found in your tables.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("The found table was not a data set.");
            }
            catch (TableColumnException e)
            {
                return BadRequest($"The column was invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not add column to data set.");
                throw;
            }
        });

    /// <summary>
    /// Removes a column from an existing table.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <param name="columnName">The name of the column.</param>
    [HttpDelete("dataset/{tableId}/{columnName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> RemoveColumnFromDataSet([FromRoute] Guid tableId, [FromRoute] string columnName)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.RemoveDataSetColumn(user, tableId, columnName);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return NotFound("The table was not found in your tables.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("The found table was not a data set.");
            }
            catch (TableColumnException e)
            {
                return BadRequest($"The column was invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not remove column from data set.");
                throw;
            }
        });

    /// <summary>
    /// Queries a collection of all sheets that the user may access.
    /// </summary>
    /// <returns>A collection of all accessible sheets.</returns>
    [HttpGet("sheet")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsAll()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries a collection of all sheets that the user owns.
    /// </summary>
    /// <returns>A collection of all of the user's sheets.</returns>
    [HttpGet("sheet/own")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsOwn()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Where(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries the number of all sheets that the user owns.
    /// </summary>
    /// <returns>The number of all of the user's sheets.</returns>
    [HttpGet("sheet/own/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsOwnCount()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Count(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries the remaining number of sheets that the user can create.
    /// </summary>
    /// <returns>The remaining number of the user's sheets.</returns>
    [HttpGet("sheet/own/remaining")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsOwnRemaining()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(_userProc.MaxSheetsPerUser - user.OwnTables.Count(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries a collection of other people's sheets that the user may access.
    /// </summary>
    /// <returns>A collection of all of the sheets shared with the user.</returns>
    [HttpGet("sheet/collaborations")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsCollaborations()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.Collaborations.Where(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries a single sheet that a user may access.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <returns>The table reference of the sheet.</returns>
    [HttpGet("sheet/{tableId}")]
    [ProducesResponseType(typeof(Table), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> MySheet([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                return Ok(await _userProc.GetSheet(user, tableId));
            }
            catch (TableNotFoundException)
            {
                return NotFound("No table with this ID was found.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("Found table of different type.");
            }
        });

    /// <summary>
    /// Creates a new sheet for a user.
    /// </summary>
    /// <param name="shModel">The definition of the sheet.</param>
    /// <returns>The table reference for the new sheet.</returns>
    [HttpPost("sheet")]
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
                table = _userProc.PrepareSheet(user, table);
                await _userProc.CreateSheet(user, table);
                return Created("/api/data/sheet/" + table.TableId, table);
            }
            catch (TableLimitExceededException)
            {
                return BadRequest("You cannot create any more additional sheets.");
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
                _logger.LogError(e, "Could not create sheet.");
                throw;
            }
        });

    /// <summary>
    /// Deletes the sheet of a user.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    [HttpDelete("sheet/{tableId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteSheet([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.DeleteSheet(user, tableId);
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

    /// <summary>
    /// Queries a collection of all tables (datasets and sheets) that the user may access.
    /// </summary>
    /// <returns>A collection of all accessible tables.</returns>
    [HttpGet("table")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyTablesAll()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Concat(user.Collaborations)));

    /// <summary>
    /// Queries a collection of all tables (datasets and sheets) that the user owns.
    /// </summary>
    /// <returns>A collection of all of the user's tables.</returns>
    [HttpGet("table/own")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyTablesOwn()
        => await AuthenticateAndRun(_authProc, User, user => Ok(user.OwnTables));

    /// <summary>
    /// Queries a collection of other people's tables (datasets and sheets) that the user may access.
    /// </summary>
    /// <returns>A collection of all of the tables shared with the user.</returns>
    [HttpGet("table/collaborations")]
    [ProducesResponseType(typeof(IEnumerable<Table>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyTablesCollaborations()
        => await AuthenticateAndRun(_authProc, User, user => Ok(user.Collaborations));

    /// <summary>
    /// Queries a collection of collaborators for a table that the user posesses.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <returns>A collection of all collaborators for this table.</returns>
    [HttpGet("table/{tableId}/collaborator")]
    [ProducesResponseType(typeof(ICollection<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> GetCollaborators([FromRoute] Guid tableId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                return Ok(await _userProc.GetCollaborators(user, tableId));
            }
            catch (TableNotFoundException)
            {
                return NotFound("Table was not found.");
            }
        });

    /// <summary>
    /// Adds a user as a collaborator to a table.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <param name="userId">The collaborator's <see cref="User.UserId"/>.</param>
    [HttpPost("table/{tableId}/collaborator/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> AddCollaborator([FromRoute] Guid tableId, [FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.AddCollaborator(user, tableId, userId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return NotFound("This table does not exist.");
            }
            catch (UserNotFoundException)
            {
                return NotFound("This user does not exist.");
            }
            catch (TableCollaboratorException e)
            {
                return BadRequest($"Collaborator was invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while adding collaborator.");
                throw;
            }
        });

    /// <summary>
    /// Removes a user as a collaborator from a table.
    /// </summary>
    /// <param name="tableId">The table reference ID.</param>
    /// <param name="userId">The collaborator's <see cref="User.UserId"/>.</param>
    [HttpDelete("table/{tableId}/collaborator/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> RemoveCollaborator([FromRoute] Guid tableId, [FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.RemoveCollaborator(user, tableId, userId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return NotFound("This table does not exist.");
            }
            catch (UserNotFoundException)
            {
                return NotFound("This user does not exist.");
            }
            catch (TableCollaboratorException e)
            {
                return BadRequest($"Collaborator was invalid: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while removing collaborator.");
                throw;
            }
        });
}
