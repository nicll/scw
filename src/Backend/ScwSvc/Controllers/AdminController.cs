using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<DataSetController> _logger;
        private readonly DbStoreContext _db;

        public AdminController(ILogger<DataSetController> logger, DbStoreContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("user")]
        [EnableQuery]
        public IQueryable<User> AllUsers()
            => _db.Users;

        [HttpGet("table")]
        [EnableQuery]
        public IQueryable<TableRef> AllTables()
            => _db.TableRefs;

        [HttpGet("dataset")]
        [EnableQuery]
        public IQueryable<TableRef> AllDataSets()
            => _db.TableRefs.Where(t => t.Type == TableType.DataSet);

        [HttpGet("sheet")]
        [EnableQuery]
        public IQueryable<TableRef> AllSheets()
            => _db.TableRefs.Where(t => t.Type == TableType.Sheet);

        [HttpPost("dataset")]
        [AuthorizeRoles(nameof(UserRole.Admin))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async ValueTask<IActionResult> CreateDataSet([FromBody] CreateDataSetModel dsModel)
        {
            var ownerInfo = GetUserIdAsGuidAndStringOrNull(User);

            if (!ownerInfo.HasValue)
                return Unauthorized("You are logged in with an invalid user.");

            var user = await _db.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existant user.");

            _logger.LogInformation("Create dataset: user=\"" + ownerInfo.Value.idStr + "\"name=" + dsModel.DisplayName);

            var newDsId = Guid.NewGuid();
            await _db.TableRefs.AddAsync(new TableRef()
            {
                TableRefId = newDsId,
                Type = TableType.DataSet,
                DisplayName = dsModel.DisplayName,
                Owner = user,
                LookupName = Guid.NewGuid(),
                Columns = ConvertColumns(dsModel.Columns, newDsId)
            });

            await _db.SaveChangesAsync();
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

            var user = await _db.Users.FindAsync(ownerInfo.Value.id).ConfigureAwait(false);

            if (user is null)
                return Unauthorized("You are logged in with a non-existant user.");

            _logger.LogInformation("Create dataset: user=\"" + ownerInfo.Value.idStr + "\"name=" + shModel.DisplayName);

            var newShId = Guid.NewGuid();
            await _db.TableRefs.AddAsync(new TableRef()
            {
                TableRefId = newShId,
                Type = TableType.Sheet,
                DisplayName = shModel.DisplayName,
                Owner = user,
                LookupName = Guid.NewGuid()
            });

            await _db.SaveChangesAsync();
            return Ok();
        }

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
