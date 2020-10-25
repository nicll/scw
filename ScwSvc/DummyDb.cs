using System;
using ScwSvc.Models;

namespace ScwSvc
{
    public static class DummyDb
    {
        private static readonly Random rnd = new Random();

        public static readonly Student[] Students;

        public static readonly School[] Schools;

        static DummyDb()
        {
            Schools = new[]
            {
                CreateNewSchool("School A", "Address A"),
                CreateNewSchool("School B", "Address B")
            };

            Students = new[]
            {
                CreateNewStudent("Florian Brunner", 123, Schools[0].SchoolId),
                CreateNewStudent("Nicolas Klement", 234, Schools[0].SchoolId),
                CreateNewStudent("Felix Linhart", 345, Schools[1].SchoolId),
                CreateNewStudent("Jakob Hofer", 456, Schools[1].SchoolId)
            };
        }

        private static Student CreateNewStudent(string name, int score, Guid schoolId)
        {
            return new Student
            {
                StudentId = Guid.NewGuid(),
                SchoolId = schoolId,
                Name = name,
                Score = score
            };
        }

        private static School CreateNewSchool(string name, string address)
        {
            return new School
            {
                SchoolId = Guid.NewGuid(),
                Name = name,
                Address = address
            };
        }
    }
}
