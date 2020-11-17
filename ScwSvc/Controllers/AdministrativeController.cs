using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
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
    public class AdministrativeController : ControllerBase
    {
        private readonly ILogger<DataSetController> _logger;
        private readonly DbStoreContext _db;

        public AdministrativeController(ILogger<DataSetController> logger)
        {
            _logger = logger;
            _db = new DbStoreContext();
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("[action]")]
        [EnableQuery]
        public async ValueTask<ICollection<User>> AllUsers() => await _db.Users.ToListAsync();

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("[action]")]
        [EnableQuery]
        public async ValueTask<ICollection<TableRef>> AllTables() => await _db.TableRefs.ToListAsync();
    }
}
