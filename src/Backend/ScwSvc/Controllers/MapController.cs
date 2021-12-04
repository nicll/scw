using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Repositories;
using ScwSvc.SvcModels;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MapController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DbSysContext _sysDb;

        public MapController(ILogger<UserController> logger, DbSysContext sysDb)
        {
            _logger = logger;
            _sysDb = sysDb;
        }

        [HttpGet("id2name/{userId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> UserIdToName([FromRoute] Guid userId)
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var otherUser = await _sysDb.GetUserById(userId);

            if (otherUser is null)
                return NotFound("User could not be found.");

            return Ok(otherUser.Name);
        }

        [HttpGet("name2id/{name}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async ValueTask<IActionResult> UserNameToId([FromRoute] string name)
        {
            var userInfo = GetUserIdAsGuidOrNull(User);

            if (!userInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _sysDb.GetUserById(userInfo.Value);

            if (user is null)
                return Unauthorized("You are logged in with a non-existent user.");

            var otherUser = await _sysDb.GetUserByName(name);

            if (otherUser is null)
                return NotFound("User could not be found.");

            return Ok(otherUser.UserId.ToString());
        }
    }
}
