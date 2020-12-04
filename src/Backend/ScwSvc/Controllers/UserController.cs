using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Utils;

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

        [HttpGet("dataset")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MyDataSets()
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.Users.Include(u => u.OwnTables)
                .FirstOrDefaultAsync(u => u.UserId == ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Where(t => t.Type == TableType.DataSet));
        }

        [HttpPost("dataset")]
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

            if (user.OwnTables.Count > MaxDataSetsPerUser)
                return Forbid("You cannot own more than " + MaxDataSetsPerUser + " data sets at any time.");

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

        [HttpGet("sheet")]
        [ProducesResponseType(typeof(IEnumerable<TableRef>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> MySheets()
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.Users.Include(u => u.OwnTables)
                .FirstOrDefaultAsync(u => u.UserId == ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            return Ok(user.OwnTables.Where(t => t.Type == TableType.Sheet));
        }

        [HttpPost("sheet")]
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

            if (user.OwnTables.Count > MaxSheetsPerUser)
                return Forbid("You cannot own more than " + MaxSheetsPerUser + " sheets at any time.");

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

        // ToDo: copied from AdminController, move to separate helper class to avoid duplication
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
