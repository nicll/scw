using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Repositories;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Globals;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers
{
    [Route("api/graphql")]
    [ApiController]
    [Authorize]
    public class GraphQLTableController : ControllerBase
    {
        private readonly ILogger<GraphQLTableController> _logger;
        private readonly DbSysContext _db;

        public GraphQLTableController(ILogger<GraphQLTableController> logger, DbSysContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        public IActionResult GeneralRedirect()
            => RedirectPreserveMethod(PostgraphileBaseUrl);

        /// <summary>
        /// Queries the <see cref="TableRef.LookupName"/> for a data set's <see cref="TableRef.TableRefId"/>.
        /// </summary>
        /// <param name="tableRefId">The incoming <see cref="TableRef.TableRefId"/>.</param>
        /// <returns>The corresponding <see cref="TableRef.LookupName"/>.</returns>
        [HttpGet("dataset/{tableRefId}/lookup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async ValueTask<IActionResult> GetDataSetLookupName([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.GetUserById(ownerInfo.Value.id);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.DataSet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a data set.");

            _logger.LogInformation("Data set access: user=\"" + user.UserId + "\"; tableRefId=\"" + tableRef.TableRefId + "\"");

            return Ok(tableRef.LookupName.ToNameString());
        }

        /// <summary>
        /// Queries the <see cref="TableRef.LookupName"/> for a sheet's <see cref="TableRef.TableRefId"/>.
        /// </summary>
        /// <param name="tableRefId">The incoming <see cref="TableRef.TableRefId"/>.</param>
        /// <returns>The corresponding <see cref="TableRef.LookupName"/>.</returns>
        [HttpGet("sheet/{tableRefId}/lookup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async ValueTask<IActionResult> GetSheetLookupName([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.GetUserById(ownerInfo.Value.id);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return this.Forbidden("You are not authorized to view this table or it does not exist.");

            if (tableRef.TableType != TableType.Sheet)
                return BadRequest("Tried to access a " + tableRef.TableType + " as a sheet.");

            _logger.LogInformation("Data set access: user=\"" + user.UserId + "\"; tableRefId=\"" + tableRef.TableRefId + "\"");

            return Ok(tableRef.LookupName.ToNameString());
        }
    }
}
