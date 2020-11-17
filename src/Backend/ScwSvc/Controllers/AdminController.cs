using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScwSvc.Controllers
{
    [Route("api/[controller]")]
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

        [AuthorizeRoles(nameof(UserRole.Manager), nameof(UserRole.Admin))]
        [HttpGet("[action]")]
        [EnableQuery]
        public async ValueTask<ICollection<User>> AllUsers() => await _db.Users.ToListAsync();

        [AuthorizeRoles(nameof(UserRole.Manager), nameof(UserRole.Admin))]
        [HttpGet("[action]")]
        [EnableQuery]
        public async ValueTask<ICollection<TableRef>> AllTables() => await _db.TableRefs.ToListAsync();
    }
}
