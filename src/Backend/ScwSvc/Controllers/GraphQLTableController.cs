using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Exceptions;
using ScwSvc.Models;
using ScwSvc.Procedures.Interfaces;
using static ScwSvc.Globals;
using static ScwSvc.Utils.Authentication;

namespace ScwSvc.Controllers;

[Route("api/graphql")]
[ApiController]
[Authorize]
public class GraphQLTableController : ControllerBase
{
    private readonly ILogger<GraphQLTableController> _logger;
    private readonly IAuthProcedures _authProc;
    private readonly IGraphQLTableProcedures _graphqlProc;

    public GraphQLTableController(ILogger<GraphQLTableController> logger, IAuthProcedures authProc, IGraphQLTableProcedures graphqlProc)
    {
        _logger = logger;
        _authProc = authProc;
        _graphqlProc = graphqlProc;
    }

    [HttpPost]
    public IActionResult GeneralRedirect()
        => RedirectPreserveMethod(PostgraphileBaseUrl);

    /// <summary>
    /// Queries the <see cref="Table.LookupName"/> for a data set's <see cref="Table.TableId"/>.
    /// </summary>
    /// <param name="tableRefId">The incoming <see cref="Table.TableId"/>.</param>
    /// <returns>The corresponding <see cref="Table.LookupName"/>.</returns>
    [HttpGet("dataset/{tableRefId}/lookup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> GetDataSetLookupName([FromRoute] Guid tableRefId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                return Ok((await _graphqlProc.GetDataSetLookupName(user, tableRefId)).ToSimplifiedFormat());
            }
            catch (TableNotFoundException)
            {
                return NotFound("Table was not found.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("Table was not a data set.");
            }
        });

    /// <summary>
    /// Queries the <see cref="Table.LookupName"/> for a sheet's <see cref="Table.TableId"/>.
    /// </summary>
    /// <param name="tableRefId">The incoming <see cref="Table.TableId"/>.</param>
    /// <returns>The corresponding <see cref="Table.LookupName"/>.</returns>
    [HttpGet("sheet/{tableRefId}/lookup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
    public async ValueTask<IActionResult> GetSheetLookupName([FromRoute] Guid tableRefId)
        => await AuthenticateAndRun(_authProc, User, async user =>
        {
            try
            {
                return Ok((await _graphqlProc.GetSheetLookupName(user, tableRefId)).ToSimplifiedFormat());
            }
            catch (TableNotFoundException)
            {
                return NotFound("Table was not found.");
            }
            catch (TableMismatchException)
            {
                return BadRequest("Table was not a sheet.");
            }
        });
}
