using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Utils;

namespace ScwSvc.Controllers
{
    /// <summary>
    /// Provides administrative features for admins and managers.
    /// Managers generally only have read-only access to everything.
    /// </summary>
    [Route("api/[controller]")]
    [AuthorizeRoles(nameof(UserRole.Admin), nameof(UserRole.Manager))]
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
            if (await _sysDb.Users.FindAsync(userId).ConfigureAwait(false) is User user)
                return Ok(user);

            return NotFound("User was not found.");
        }

        [HttpDelete("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            if (await _sysDb.Users.FindAsync(userId).ConfigureAwait(false) is User user)
            {
                await _sysDb.TableRefs.Where(t => t.Collaborators.Contains(user)).ForEachAsync(t => t.Collaborators.Remove(user));
                _sysDb.TableRefs.RemoveRange(user.OwnTables);
                _sysDb.Users.Remove(user);
                await _sysDb.SaveChangesAsync();
                return Ok();
            }

            return NotFound("User was not found.");
        }

        [HttpGet("user/{userId}/table")]
        [ProducesResponseType(typeof(IQueryable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetUserTables([FromRoute] Guid userId)
        {
            if (!(await _sysDb.Users.FindAsync(userId).ConfigureAwait(false) is User user))
                return NotFound("User was not found.");

            return Ok(_sysDb.TableRefs.Where(t => t.OwnerUserId == userId).Select(t => t.TableRefId));
        }

        [HttpGet("user/{userId}/collaboration")]
        [ProducesResponseType(typeof(IQueryable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> GetUserCollaborations([FromRoute] Guid userId)
        {
            if (!(await _sysDb.Users.FindAsync(userId).ConfigureAwait(false) is User user))
                return NotFound("User was not found.");

            return Ok(_sysDb.TableRefs.Where(t => t.Collaborators.Contains(user)).Select(t => t.TableRefId));
        }

        [HttpGet("table")]
        [EnableQuery]
        public IQueryable<TableRef> GetTables()
            => _sysDb.TableRefs;

        [HttpGet("dataset")]
        [EnableQuery]
        public IQueryable<TableRef> GetDataSets()
            => _sysDb.TableRefs.Where(t => t.Type == TableType.DataSet).Include(d => d.Columns);

        [HttpGet("sheet")]
        [EnableQuery]
        public IQueryable<TableRef> GetSheets()
            => _sysDb.TableRefs.Where(t => t.Type == TableType.Sheet);

        [HttpPost("dataset")]
        [AuthorizeRoles(nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateDataSet([FromBody] CreateDataSetModel dsModel)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            if (dsModel.Columns.Length < 1)
                return BadRequest("No columns specified.");

            _logger.LogInformation("Create dataset: user=\"" + ownerInfo.Value.idStr + "\"; name=" + dsModel.DisplayName);

            var newDsId = Guid.NewGuid();
            var newTable = new TableRef()
            {
                TableRefId = newDsId,
                Type = TableType.DataSet,
                DisplayName = dsModel.DisplayName,
                Owner = user,
                LookupName = Guid.NewGuid(),
                Columns = ConvertColumns(dsModel.Columns, newDsId)
            };

            await _sysDb.TableRefs.AddAsync(newTable);
            await Interactors.DynDbInteractor.CreateDataSet(newTable, _dynDb);

            await _sysDb.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("dataset/{tableRefId}")]
        [AuthorizeRoles(nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> RemoveDataSet([FromRoute] Guid tableRefId)
        {
            var userInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.Users.FindAsync(userInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var table = await _sysDb.TableRefs.FindAsync(tableRefId);

            if (table is null)
                return NotFound("This data set does not exist.");

            if (table.Type != TableType.DataSet)
                return BadRequest("Incorrect table type.");

            _logger.LogInformation("Remove dataset: user=\"" + userInfo.Value.idStr + "\"; TableRefId=" + tableRefId + "; DisplayName=" + table.DisplayName + "; OwnerUserId=" + table.OwnerUserId);

            _sysDb.Remove(table);
            await Interactors.DynDbInteractor.RemoveDataSet(table, _dynDb);
            await _sysDb.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("sheet")]
        [AuthorizeRoles(nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateSheet([FromBody] CreateSheetModel shModel)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            _logger.LogInformation("Create sheet: user=\"" + ownerInfo.Value.idStr + "\"; name=" + shModel.DisplayName);

            var newShId = Guid.NewGuid();
            var newTable = new TableRef()
            {
                TableRefId = newShId,
                Type = TableType.Sheet,
                DisplayName = shModel.DisplayName,
                Owner = user,
                LookupName = Guid.NewGuid()
            };

            await _sysDb.TableRefs.AddAsync(newTable);
            await Interactors.DynDbInteractor.CreateSheet(newTable, _dynDb);

            await _sysDb.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("sheet/{tableRefId}")]
        [AuthorizeRoles(nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> RemoveSheet([FromRoute] Guid tableRefId)
        {
            var userInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.Users.FindAsync(userInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var table = await _sysDb.TableRefs.FindAsync(tableRefId);

            if (table is null)
                return NotFound("This sheet does not exist.");

            if (table.Type != TableType.Sheet)
                return BadRequest("Incorrect table type.");

            _logger.LogInformation("Remove sheet: user=\"" + userInfo.Value.idStr + "\"; TableRefId=" + tableRefId + "; DisplayName=" + table.DisplayName + "; OwnerUserId=" + table.OwnerUserId);

            _sysDb.Remove(table);
            await Interactors.DynDbInteractor.RemoveSheet(table, _dynDb);
            await _sysDb.SaveChangesAsync();
            return Ok();
        }

        // ToDo: move to helper class
        private static DataSetColumn[] ConvertColumns(CreateDataSetModel.ColumnDefinition[] definition, Guid tableRefId)
        {
            var result = new DataSetColumn[definition.Length];

            for (int i = 0; i < definition.Length; ++i)
            {
                result[i] = new DataSetColumn()
                {
                    TableRefId = tableRefId,
                    Position = (byte)i,
                    Name = definition[i].Name,
                    Nullable = definition[i].Nullable,
                    Type = definition[i].Type
                };
            }

            return result;
        }
    }
}
