using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ScwSvc.Globals;
using static ScwSvc.Utils;

namespace ScwSvc.Controllers
{
    [Route("api/table")]
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

        [HttpGet("{tableRefId}")]
        [HttpPost("{tableRefId}")]
        [HttpPatch("{tableRefId}")]
        [HttpDelete("{tableRefId}")]
        public async ValueTask<IActionResult> GetTable([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.Users.Include(u => u.OwnTables).Include(u => u.Collaborations)
                .FirstOrDefaultAsync(u => u.UserId == ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return Forbid("You are not authorized to view this table or it does not exist.");

            return RedirectPreserveMethod(PostgrestBaseUrl + tableRef.LookupName.ToNameString() + "?" + HttpContext.Request.QueryString);
        }

        [HttpGet("2/{tableRefId}")]
        public async ValueTask<IActionResult> GetTable2([FromRoute] Guid tableRefId)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.Users.Include(u => u.OwnTables).Include(u => u.Collaborations)
                .FirstOrDefaultAsync(u => u.UserId == ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var tableRef = user.OwnTables.FirstOrDefault(t => t.TableRefId == tableRefId)
                ?? user.Collaborations.FirstOrDefault(t => t.TableRefId == tableRefId);

            if (tableRef is null)
                return Forbid("You are not authorized to view this table or it does not exist.");

            throw null;
        }
    }
}
