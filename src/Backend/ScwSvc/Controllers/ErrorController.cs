﻿using Microsoft.AspNetCore.Mvc;

namespace ScwSvc.Controllers;

[ApiController]
public class ErrorController : ControllerBase
{
    [HttpGet("/error")]
    [HttpPost("/error")]
    [HttpPut("/error")]
    [HttpPatch("/error")]
    [HttpDelete("/error")]
    public IActionResult Error() => Problem();
}
