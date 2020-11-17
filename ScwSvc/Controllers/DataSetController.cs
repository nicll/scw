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

        public DataSetController(ILogger<DataSetController> logger)
        {
            _logger = logger;
        }
    }
}
