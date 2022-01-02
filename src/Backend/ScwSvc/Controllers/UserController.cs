using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Procedures.Interfaces;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.SvcModels;
using static ScwSvc.Utils.Authentication;
using static ScwSvc.Utils.DataConversion;

namespace ScwSvc.Controllers;

[Route("api/my")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IAuthProcedures _authProc;
    private readonly IUserProcedures _userProc;
    private readonly IAdminProcedures _tableProc;
    // following is TEMPORARY
    public const int MaxDataSetsPerUser = 20;
    public const int MaxSheetsPerUser = 20;

    public UserController(ILogger<UserController> logger, IAuthProcedures authProc, IUserProcedures userProc, IAdminProcedures tableProc)
    {
        _logger = logger;
        _authProc = authProc;
        _userProc = userProc;
        _tableProc = tableProc;
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
                return BadRequest("This user does not exist.");
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest("A user with this name already exists.");
            }
            catch (UserChangeException e)
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
                return BadRequest("This user does not exist.");
            }
            catch (UserChangeException e)
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
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsAll()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries a collection of all data sets that the user owns.
    /// </summary>
    /// <returns>A collection of all of the user's data sets.</returns>
    [HttpGet("dataset/own")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
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
            user => Ok(MaxDataSetsPerUser - user.OwnTables.Count(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries a collection of other people's data sets that the user may access.
    /// </summary>
    /// <returns>A collection of all of the data sets shared with the user.</returns>
    [HttpGet("dataset/collaborations")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyDataSetsCollaborations()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.Collaborations.Where(t => t.TableType == TableType.DataSet)));

    /// <summary>
    /// Queries a single data set that a user may access.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    /// <returns>The table reference of the data set.</returns>
    [HttpGet("dataset/{tableRefId}")]
    [ProducesResponseType(typeof(TableRef), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> MyDataSet([FromRoute] Guid tableRefId)
    {
        var userInfo = GetUserIdAsGuidOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        var tableRef = user.OwnTables.FirstOrDefault(t => t.TableId == tableRefId)
            ?? user.Collaborations.FirstOrDefault(t => t.TableId == tableRefId);

        if (tableRef is null)
            return this.Forbidden("You are not allowed to access this table or it does not exist.");

        if (tableRef.TableType != TableType.DataSet)
            return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

        return Ok(tableRef);
    }

    /// <summary>
    /// Creates a new data set for a user.
    /// </summary>
    /// <param name="dsModel">The definition of the data set.</param>
    /// <returns>The table reference for the new data set.</returns>
    [HttpPost("dataset")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> CreateDataSet([FromBody] CreateDataSet dsModel)
    {
        var userInfo = GetUserIdAsGuidAndStringOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value.id);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        if (user.OwnTables.Count > MaxDataSetsPerUser)
            return this.Forbidden("You cannot own more than " + MaxDataSetsPerUser + " data sets at any time.");

        _logger.LogInformation("Create dataset: user=\"" + userInfo.Value.idStr + "\"; name=" + dsModel.DisplayName);

        try
        {
            var newDsId = Guid.NewGuid();
            var newTable = new TableRef()
            {
                TableId = newDsId,
                TableType = TableType.DataSet,
                DisplayName = dsModel.DisplayName,
                Owner = user,
                LookupName = Guid.NewGuid(),
                Columns = ConvertColumns(dsModel.Columns, newDsId)
            };

            await _sysDb.AddTable(newTable);
            await _dynDb.CreateTable(newTable);
            await _sysDb.SaveChanges();

            return Created("/api/data/dataset/" + newDsId, newTable);
        }
        catch (InvalidTableException e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Deletes the data set of a user.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    [HttpDelete("dataset/{tableRefId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> DeleteDataSet([FromRoute] Guid tableRefId)
    {
        var userInfo = GetUserIdAsGuidOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        var tableRef = user.OwnTables.FirstOrDefault(t => t.TableId == tableRefId);

        if (tableRef is null)
            return this.Forbidden("You are not authorized to view this table or it does not exist.");

        if (tableRef.TableType != TableType.DataSet)
            return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

        await _sysDb.RemoveTable(tableRef);
        await _dynDb.RemoveTable(tableRef);
        await _sysDb.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// Adds a column to an existing table.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    /// <param name="columnName">The name of the new column.</param>
    /// <param name="column">The definition of the new column.</param>
    [HttpPost("dataset/{tableRefId}/{columnName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> AddColumnToDataSet([FromRoute] Guid tableRefId, [FromRoute] string columnName, [FromBody] ColumnDefinition column)
    {
        var userInfo = GetUserIdAsGuidOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        var tableRef = user.OwnTables.FirstOrDefault(t => t.TableId == tableRefId);

        if (tableRef is null)
            return this.Forbidden("You are not authorized to view this table or it does not exist.");

        if (tableRef.TableType != TableType.DataSet)
            return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

        if (columnName != column.Name)
            return BadRequest("Column name does not match.");

        if (tableRef.Columns.Count >= Byte.MaxValue)
            throw new InvalidTableException("Too many columns in table.");

        if (!tableRef.Columns.Select(c => c.Name).Append(column.Name).AllUnique())
            throw new InvalidTableException("Column names not unique.");

        var dsColumn = new DataSetColumn()
        {
            TableRefId = tableRef.TableId,
            TableRef = tableRef,
            Name = column.Name,
            Type = column.Type,
            Nullable = column.Nullable,
            Position = (byte)(tableRef.Columns.Max(c => c.Position) + 1)
        };

        tableRef.Columns.Add(dsColumn);

        await _sysDb.ModifyTable(tableRef);
        await _dynDb.AddDataSetColumn(tableRef, dsColumn);
        await _sysDb.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// Removes a column from an existing table.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    /// <param name="columnName">The name of the column.</param>
    [HttpDelete("dataset/{tableRefId}/{columnName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> RemoveColumnFromDataSet([FromRoute] Guid tableRefId, [FromRoute] string columnName)
    {
        var userInfo = GetUserIdAsGuidOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        var tableRef = user.OwnTables.FirstOrDefault(t => t.TableId == tableRefId);

        if (tableRef is null)
            return this.Forbidden("You are not authorized to view this table or it does not exist.");

        if (tableRef.TableType != TableType.DataSet)
            return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

        if (tableRef.Columns.Count(c => c.Name == columnName) != 1)
            return BadRequest("Column does not exist.");

        await _sysDb.ModifyTable(tableRef);
        await _dynDb.RemoveDataSetColumn(tableRef, columnName);
        await _sysDb.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// Queries a collection of all sheets that the user may access.
    /// </summary>
    /// <returns>A collection of all accessible sheets.</returns>
    [HttpGet("sheet")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsAll()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries a collection of all sheets that the user owns.
    /// </summary>
    /// <returns>A collection of all of the user's sheets.</returns>
    [HttpGet("sheet/own")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
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
            user => Ok(MaxSheetsPerUser - user.OwnTables.Count(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries a collection of other people's sheets that the user may access.
    /// </summary>
    /// <returns>A collection of all of the sheets shared with the user.</returns>
    [HttpGet("sheet/collaborations")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MySheetsCollaborations()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.Collaborations.Where(t => t.TableType == TableType.Sheet)));

    /// <summary>
    /// Queries a single sheet that a user may access.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    /// <returns>The table reference of the sheet.</returns>
    [HttpGet("sheet/{tableRefId}")]
    [ProducesResponseType(typeof(TableRef), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> MySheet([FromRoute] Guid tableRefId)
    {
        var userInfo = GetUserIdAsGuidOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        var tableRef = user.OwnTables.FirstOrDefault(t => t.TableId == tableRefId)
            ?? user.Collaborations.FirstOrDefault(t => t.TableId == tableRefId);

        if (tableRef is null)
            return this.Forbidden("You are not allowed to access this table or it does not exist.");

        if (tableRef.TableType != TableType.Sheet)
            return BadRequest("Tried to access a " + tableRef.TableType + " as a sheet.");

        return Ok(tableRef);
    }

    /// <summary>
    /// Creates a new sheet for a user.
    /// </summary>
    /// <param name="shModel">The definition of the sheet.</param>
    /// <returns>The table reference for the new sheet.</returns>
    [HttpPost("sheet")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> CreateSheet([FromBody] CreateSheet shModel)
    {
        var userInfo = GetUserIdAsGuidAndStringOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value.id);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        if (user.OwnTables.Count > MaxSheetsPerUser)
            return this.Forbidden("You cannot own more than " + MaxSheetsPerUser + " sheets at any time.");

        _logger.LogInformation("Create sheet: user=\"" + userInfo.Value.idStr + "\"; name=" + shModel.DisplayName);

        var newShId = Guid.NewGuid();
        var newTable = new TableRef()
        {
            TableId = newShId,
            TableType = TableType.Sheet,
            DisplayName = shModel.DisplayName,
            Owner = user,
            LookupName = Guid.NewGuid()
        };

        await _sysDb.AddTable(newTable);
        await _dynDb.CreateTable(newTable);
        await _sysDb.SaveChanges();

        return Created("/api/data/sheet/" + newShId, newTable);
    }

    /// <summary>
    /// Deletes the sheet of a user.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    [HttpDelete("sheet/{tableRefId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> DeleteSheet([FromRoute] Guid tableRefId)
    {
        var userInfo = GetUserIdAsGuidOrNull(User);

        if (!userInfo.HasValue)
            return Unauthorized("You are logged in with an invalid user.");

        var user = await _sysDb.GetUserById(userInfo.Value);

        if (user is null)
            return Unauthorized("You are logged in with a non-existent user.");

        var tableRef = user.OwnTables.FirstOrDefault(t => t.TableId == tableRefId);

        if (tableRef is null)
            return this.Forbidden("You are not authorized to view this table or it does not exist.");

        if (tableRef.TableType != TableType.Sheet)
            return BadRequest("Tried to access a " + tableRef.TableType + " as a sheet.");

        await _sysDb.RemoveTable(tableRef);
        await _dynDb.RemoveTable(tableRef);
        await _sysDb.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// Queries a collection of all tables (datasets and sheets) that the user may access.
    /// </summary>
    /// <returns>A collection of all accessible tables.</returns>
    [HttpGet("table")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyTablesAll()
        => await AuthenticateAndRun(_authProc, User,
            user => Ok(user.OwnTables.Concat(user.Collaborations)));

    /// <summary>
    /// Queries a collection of all tables (datasets and sheets) that the user owns.
    /// </summary>
    /// <returns>A collection of all of the user's tables.</returns>
    [HttpGet("table/own")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyTablesOwn()
        => await AuthenticateAndRun(_authProc, User, user => Ok(user.OwnTables));

    /// <summary>
    /// Queries a collection of other people's tables (datasets and sheets) that the user may access.
    /// </summary>
    /// <returns>A collection of all of the tables shared with the user.</returns>
    [HttpGet("table/collaborations")]
    [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> MyTablesCollaborations()
        => await AuthenticateAndRun(_authProc, User, user => Ok(user.Collaborations));

    /// <summary>
    /// Queries a collection of collaborators for a table that the user posesses.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    /// <returns>A collection of all collaborators for this table.</returns>
    [HttpGet("table/{tableRefId}/collaborator")]
    [ProducesResponseType(typeof(ICollection<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> GetCollaborators([FromRoute] Guid tableRefId)
        => await AuthenticateAndRun(_authProc, User, async user => Ok(await _userProc.GetCollaborators(user, tableRefId)));

    /// <summary>
    /// Adds a user as a collaborator to a table.
    /// </summary>
    /// <param name="tableRefId">The table reference ID.</param>
    /// <param name="userId">The collaborator's <see cref="User.UserId"/>.</param>
    [HttpPost("table/{tableRefId}/collaborator/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> AddCollaborator([FromRoute] Guid tableRefId, [FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.AddCollaborator(user, tableRefId, userId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return BadRequest("This table does not exist.");
            }
            catch (UserNotFoundException)
            {
                return BadRequest("This user does not exist.");
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
    /// <param name="tableRefId">The table reference ID.</param>
    /// <param name="userId">The collaborator's <see cref="User.UserId"/>.</param>
    [HttpDelete("table/{tableRefId}/collaborator/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> RemoveCollaborator([FromRoute] Guid tableRefId, [FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                await _userProc.RemoveCollaborator(user, tableRefId, userId);
                return Ok();
            }
            catch (TableNotFoundException)
            {
                return BadRequest("This table does not exist.");
            }
            catch (UserNotFoundException)
            {
                return BadRequest("This user does not exist.");
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
