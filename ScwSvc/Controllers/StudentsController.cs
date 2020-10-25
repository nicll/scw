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
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            if (Guid.TryParse(id, out var studentId))
                return Ok(DummyDb.Students.SingleOrDefault(s => s.StudentId == studentId));

            return BadRequest();
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<Student> GetStudents()
        {
            return DummyDb.Students.AsQueryable();
        }
    }
}
