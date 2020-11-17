using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScwSvc.Models;

namespace ScwSvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSetController : ControllerBase
    {
        private readonly ILogger<DataSetController> _logger;
        private readonly DbStoreContext _db;

        public DataSetController(ILogger<DataSetController> logger, DbStoreContext db)
        {
            _logger = logger;
            _db = db;
        }
    }
}
