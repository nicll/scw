using Microsoft.AspNetCore.Mvc;
using System;

namespace ScwSvc.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}
