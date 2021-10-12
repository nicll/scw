using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Repositories;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Utils.Authentication;
using static ScwSvc.Utils.DataConversion;

namespace ScwSvc.Controllers
{
    [Route("api/my")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbSysContext _sysDb;
        private readonly DbDynContext _dynDb;

        /// <summary>
        /// Maximum amount of data sets one user may own at any time.
        /// </summary>
        /// <remarks>
        /// This value is bypassed if additional tables are assigned by an administrator.
        /// </remarks>
        public const int MaxDataSetsPerUser = 20;

        /// <summary>
        /// Maximum amount of sheets one user may own at any time.
        /// </summary>
        /// <remarks>
        /// This value is bypassed if additional tables are assigned by an administrator.
        /// </remarks>
        public const int MaxSheetsPerUser = 20;

        public UserController(ILogger<UserController> logger, DbSysContext sysDb, DbDynContext dynDb)
        {
            _logger = logger;
            _sysDb = sysDb;
            _dynDb = dynDb;
        }

        [HttpGet("username")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyUsername()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.Name);
        }

        [HttpPatch("username")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> ChangeUsername([FromBody] string username)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Currently not supported.");
            // ToDo: allow this call when username is no longer readonly

            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            if (await _sysDb.IsUsernameAssigned(username))
                return BadRequest("User with this name already exists.");

            try
            {
                await _sysDb.ModifyUser(user, username: username);
                return Ok();
            }
            catch (UserChangeException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> ChangePassword([FromBody] string password)
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            try
            {
                await _sysDb.ModifyUser(user, password: password);
                return Ok();
            }
            catch (UserChangeException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Queries a collection of all data sets that the user may access.
        /// </summary>
        /// <returns>A collection of all accessible data sets.</returns>
        [HttpGet("dataset")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyDataSetsAll()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.DataSet));
        }

        /// <summary>
        /// Queries a collection of all data sets that the user owns.
        /// </summary>
        /// <returns>A collection of all of the user's data sets.</returns>
        [HttpGet("dataset/own")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyDataSetsOwn()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Where(t => t.TableType == TableType.DataSet));
        }

        /// <summary>
        /// Queries a collection of other people's data sets that the user may access.
        /// </summary>
        /// <returns>A collection of all of the data sets shared with the user.</returns>
        [HttpGet("dataset/collaborations")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyDataSetsCollaborations()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.Collaborations.Where(t => t.TableType == TableType.DataSet));
        }

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

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

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
                    TableRefId = newDsId,
                    TableType = TableType.DataSet,
                    DisplayName = dsModel.DisplayName,
                    Owner = user,
                    LookupName = Guid.NewGuid(),
                    Columns = ConvertColumns(dsModel.Columns, newDsId)
                };

                await _sysDb.AddTable(newTable);
                await _dynDb.CreateDataSet(newTable);
                await _sysDb.SaveChangesAsync();

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

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.DataSet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

            await _sysDb.RemoveTable(tableRef);
            await _dynDb.RemoveDataSet(tableRef);
            await _sysDb.SaveChangesAsync();

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

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.DataSet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

            if (columnName != column.Name)
                return BadRequest("Column name does not match.");

            var dsColumn = await _sysDb.AddColumnToDataSet(tableRef, column, commit: false);
            await _dynDb.AddColumnToDataSet(tableRef, dsColumn);
            await _sysDb.SaveChangesAsync();

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

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.DataSet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

            if (!tableRef.Columns.Any(c => c.Name == columnName))
                return BadRequest("Column does not exist.");

            await _sysDb.RemoveColumnFromDataSet(tableRef, columnName, commit: false);
            await _dynDb.RemoveColumnFromDataSet(tableRef, columnName);
            await _sysDb.SaveChangesAsync();

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
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.Sheet));
        }

        /// <summary>
        /// Queries a collection of all sheets that the user owns.
        /// </summary>
        /// <returns>A collection of all of the user's sheets.</returns>
        [HttpGet("sheet/own")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MySheetsOwn()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Where(t => t.TableType == TableType.Sheet));
        }

        /// <summary>
        /// Queries a collection of other people's sheets that the user may access.
        /// </summary>
        /// <returns>A collection of all of the sheets shared with the user.</returns>
        [HttpGet("sheet/collaborations")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MySheetsCollaborations()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.Collaborations.Where(t => t.TableType == TableType.Sheet));
        }

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

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

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
                TableRefId = newShId,
                TableType = TableType.Sheet,
                DisplayName = shModel.DisplayName,
                Owner = user,
                LookupName = Guid.NewGuid()
            };

            await _sysDb.AddTable(newTable);
            await _dynDb.CreateSheet(newTable);
            await _sysDb.SaveChangesAsync();

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

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.Sheet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a sheet.");

            await _sysDb.RemoveTable(tableRef);
            await _dynDb.RemoveSheet(tableRef);
            await _sysDb.SaveChangesAsync();

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
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Concat(user.Collaborations));
        }

        /// <summary>
        /// Queries a collection of all tables (datasets and sheets) that the user owns.
        /// </summary>
        /// <returns>A collection of all of the user's tables.</returns>
        [HttpGet("table/own")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyTablesOwn()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables);
        }

        /// <summary>
        /// Queries a collection of other people's tables (datasets and sheets) that the user may access.
        /// </summary>
        /// <returns>A collection of all of the tables shared with the user.</returns>
        [HttpGet("table/collaborations")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyTablesCollaborations()
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.Collaborations);
        }
    }
}
