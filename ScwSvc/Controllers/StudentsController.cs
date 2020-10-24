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
    [ODataRoutePrefix("Students")]
    public class StudentsController : ControllerBase
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly Student[] _students =
        {
            CreateNewStudent("Florian Brunner", 123),
            CreateNewStudent("Nicolas Klement", 234)
        };

        public StudentsController(ILogger<StudentsController> logger)
        {
            _logger = logger;
        }

        [ODataRoute]
        public IQueryable<Student> Get()
        {
            return _students.AsQueryable();
        }

        [ODataRoute("{id}")]
        public Student Get([FromODataUri] Guid id)
        {
            return _students.SingleOrDefault(s => s.Id == id);
        }

        private static Student CreateNewStudent(string name, int score)
        {
            return new Student
            {
                Id = Guid.NewGuid(),
                Name = name,
                Score = score
            };
        }
    }
}
