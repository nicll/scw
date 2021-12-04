using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.DataAccess.Interfaces;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.SvcModels;
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
        private readonly ISysDbRepository _sysDb;
        private readonly IDynDbRepository _dynDb;

        public AdminController(ILogger<AdminController> logger, ISysDbRepository sysDb, IDynDbRepository dynDb)
        {
            _logger = logger;
            _sysDb = sysDb;
            _dynDb = dynDb;
        }

        [HttpGet("user")]
        public IQueryable<User> GetUsers()
            => _sysDb.GetUsers();

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

        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async ValueTask<IActionResult> AddUser([FromBody] AuthenticationCredentials credentials)
        {
            return BadRequest("Not yet implemented.");
        }

        [HttpPut("user/{userId}/username")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> ChangeUserName([FromRoute] Guid userId, [FromBody] string username)
        {
            return BadRequest("Not yet implemented.");
        }

        [HttpPut("user/{userId}/roles")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> ChangeUserRole([FromRoute] Guid userId, [FromBody] UserRole role)
        {
            return BadRequest("Not yet implemented.");
        }

        [HttpPut("user/{userId}/password")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> ChangeUserPassword([FromRoute] Guid userId, [FromBody] string password)
        {
            return BadRequest("Not yet implemented.");
        }

        [HttpDelete("user/{userId}")]
        [Authorize(Policy = AdminOnly)]
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

            await _sysDb.SaveChanges();
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
        public IQueryable<TableRef> GetTables()
            => _sysDb.GetTables();

        [HttpGet("dataset")]
        public IQueryable<TableRef> GetDataSets()
            => _sysDb.GetTables().Where(t => t.TableType == TableType.DataSet);

        [HttpGet("sheet")]
        public IQueryable<TableRef> GetSheets()
            => _sysDb.GetTables().Where(t => t.TableType == TableType.Sheet);

        [HttpPost("dataset")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateDataSet([FromBody] CreateDataSet dsModel)
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
                await _dynDb.CreateTable(newTable);
                await _sysDb.SaveChanges();

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

            var table = await _sysDb.GetTableById(tableRefId);

            if (table is null)
                return NotFound("This data set does not exist.");

            if (table.TableType != TableType.DataSet)
                return BadRequest("Incorrect table type.");

            _logger.LogInformation("Remove dataset: user=\"" + userInfo.Value.idStr + "\"; TableRefId=" + tableRefId + "; DisplayName=" + table.DisplayName + "; OwnerUserId=" + table.OwnerUserId);

            await _sysDb.RemoveTable(table);
            await _dynDb.RemoveTable(table);
            await _sysDb.SaveChanges();

            return Ok();
        }

        [HttpPost("sheet")]
        [Authorize(Policy = AdminOnly)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateSheet([FromBody] CreateSheet shModel)
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
            await _dynDb.CreateTable(newTable);
            await _sysDb.SaveChanges();

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

            var table = await _sysDb.GetTableById(tableRefId);

            if (table is null)
                return NotFound("This sheet does not exist.");

            if (table.TableType != TableType.Sheet)
                return BadRequest("Incorrect table type.");

            _logger.LogInformation("Remove sheet: user=\"" + userInfo.Value.idStr + "\"; TableRefId=" + tableRefId + "; DisplayName=" + table.DisplayName + "; OwnerUserId=" + table.OwnerUserId);

            await _sysDb.RemoveTable(table);
            await _dynDb.RemoveTable(table);
            await _sysDb.SaveChanges();

            return Ok();
        }
    }
}
