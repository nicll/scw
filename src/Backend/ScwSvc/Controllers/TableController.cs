using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Globals;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers
{
    [Route("api/data")]
    [ApiController]
    [Authorize]
    public class TableController : ControllerBase
    {
        private readonly ILogger<TableController> _logger;
        private readonly DbSysContext _db;

        public TableController(ILogger<TableController> logger, DbSysContext db)
        {
            _logger = logger;
            _db = db;
        }

        /// <summary>
        /// Redirect to the specified PostgREST endpoint for this data set.
        /// </summary>
        /// <param name="tableRefId">The ID of the table reference.</param>
        /// <returns>Redirect to PostgREST server.</returns>
        [HttpGet("dataset/{tableRefId}")]
        [HttpPost("dataset/{tableRefId}")]
        [HttpPatch("dataset/{tableRefId}")]
        [HttpDelete("dataset/{tableRefId}")]
        [ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async ValueTask<IActionResult> GetDataSet([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.DataSet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

            _logger.LogInformation("Data set access: user=\"" + user.UserId + "\"; tableRedId=\"" + tableRef.TableRefId + "\"");

            return RedirectPreserveMethod(PostgrestBaseUrl + tableRef.LookupName.ToNameString() + "?" + HttpContext.Request.QueryString);
        }

        /// <summary>
        /// Redirect to the specified PostgREST endpoint for this sheet.
        /// </summary>
        /// <param name="tableRefId">The ID of the table reference.</param>
        /// <returns>Redirect to PostgREST server.</returns>
        [HttpGet("sheet/{tableRefId}")]
        [HttpPost("sheet/{tableRefId}")]
        [HttpPatch("sheet/{tableRefId}")]
        [HttpDelete("sheet/{tableRefId}")]
        [ProducesResponseType(StatusCodes.Status307TemporaryRedirect)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async ValueTask<IActionResult> GetSheet([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.Sheet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a sheet.");

            _logger.LogInformation("Sheet access: user=\"" + user.UserId + "\"; tableRedId=\"" + tableRef.TableRefId + "\"");

            return RedirectPreserveMethod(PostgrestBaseUrl + tableRef.LookupName.ToNameString() + "?" + HttpContext.Request.QueryString);
        }

#if DEBUG
        [HttpGet("test/{tableRefId}")]
        public async ValueTask<IActionResult> GetTable2([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            throw null;
        }
#endif
    }
}
