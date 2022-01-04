using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Procedures.Interfaces;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MapController : ControllerBase
{
    private readonly ILogger<MapController> _logger;
    private readonly IAuthProcedures _authProc;
    private readonly IMapProcedures _mapProc;

    public MapController(ILogger<MapController> logger, IAuthProcedures authProc, IMapProcedures mapProc)
    {
        _logger = logger;
        _authProc = authProc;
        _mapProc = mapProc;
    }

    [HttpGet("id2name/{userId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> UserIdToName([FromRoute] Guid userId)
        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            var name = await _mapProc.UserIdToName(userId);

            if (name is null)
                return NotFound("User could not be found.");

            return Ok(name);
        });

    [HttpGet("name2id/{name}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> UserNameToId([FromRoute] string name)

        => await AuthenticateAndRun(_authProc, User, async _ =>
        {
            var id = await _mapProc.UserNameToId(name);

            if (!id.HasValue)
                return NotFound("User could not be found.");

            return Ok(id.Value);
        });
}
