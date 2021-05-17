using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Interactors;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Globals.Authorization;
using static ScwSvc.Utils.Authentication;
using static ScwSvc.Utils.DataConversion;

namespace ScwSvc.Controllers
{
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
        private readonly DbSysContext _sysDb;
        private readonly DbDynContext _dynDb;

        public AdminController(ILogger<AdminController> logger, DbSysContext sysDb, DbDynContext dynDb)
        {
            _logger = logger;
            _sysDb = sysDb;
            _dynDb = dynDb;
        }

        [HttpGet("user")]
        [EnableQuery]
        public IQueryable<User> GetUsers()
            => _sysDb.Users;

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var user = await _sysDb.GetUserById(userId);

            if (user is null)
                return NotFound("User was not found.");

            return Ok(user);
        }

        [HttpDelete("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var user = await _sysDb.GetUserById(userId);

            if (user is null)
                return NotFound("User was not found.");

            await _sysDb.RemoveUser(user);

            foreach (var tableRef in user.OwnTables)
                await _dynDb.RemoveTable(tableRef);

            await _sysDb.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("user/{userId}/table")]
        [ProducesResponseType(typeof(ICollection<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetUserTables([FromRoute] Guid userId)
        {
            var user = await _sysDb.GetUserById(userId);

            if (user is null)
                return NotFound("User was not found.");

            return Ok(user.OwnTables);
        }

        [HttpGet("user/{userId}/collaboration")]
        [ProducesResponseType(typeof(ICollection<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetUserCollaborations([FromRoute] Guid userId)
        {
            var user = await _sysDb.GetUserById(userId);

            if (user is null)
                return NotFound("User was not found.");

            return Ok(user.Collaborations);
        }

        [HttpGet("table")]
        [EnableQuery]
        public IQueryable<TableRef> GetTables()
            => _sysDb.TableRefs;

        [HttpGet("dataset")]
        [EnableQuery]
        public IQueryable<TableRef> GetDataSets()
            => _sysDb.TableRefs.Where(t => t.TableType == TableType.DataSet);

        [HttpGet("sheet")]
        [EnableQuery]
        public IQueryable<TableRef> GetSheets()
            => _sysDb.TableRefs.Where(t => t.TableType == TableType.Sheet);

        [HttpPost("dataset")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateDataSet([FromBody] CreateDataSetModel dsModel)
        {
            var userInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value.id);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            if (dsModel.Columns.Length < 1)
                return BadRequest("No columns specified.");

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

        [HttpDelete("dataset/{tableRefId}")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> RemoveDataSet([FromRoute] Guid tableRefId)
        {
            var userInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value.id);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var table = await _sysDb.GetTableRefById(tableRefId);

            if (table is null)
                return NotFound("This data set does not exist.");

            if (table.TableType != TableType.DataSet)
                return BadRequest("Incorrect table type.");

            _logger.LogInformation("Remove dataset: user=\"" + userInfo.Value.idStr + "\"; TableRefId=" + tableRefId + "; DisplayName=" + table.DisplayName + "; OwnerUserId=" + table.OwnerUserId);

            await _sysDb.RemoveTable(table);
            await _dynDb.RemoveDataSet(table);
            await _sysDb.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("sheet")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateSheet([FromBody] CreateSheetModel shModel)
        {
            var userInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value.id);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

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

        [HttpDelete("sheet/{tableRefId}")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> RemoveSheet([FromRoute] Guid tableRefId)
        {
            var userInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value.id);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var table = await _sysDb.GetTableRefById(tableRefId);

            if (table is null)
                return NotFound("This sheet does not exist.");

            if (table.TableType != TableType.Sheet)
                return BadRequest("Incorrect table type.");

            _logger.LogInformation("Remove sheet: user=\"" + userInfo.Value.idStr + "\"; TableRefId=" + tableRefId + "; DisplayName=" + table.DisplayName + "; OwnerUserId=" + table.OwnerUserId);

            await _sysDb.RemoveTable(table);
            await _dynDb.RemoveSheet(table);
            await _sysDb.SaveChangesAsync();

            return Ok();
        }
    }
}
