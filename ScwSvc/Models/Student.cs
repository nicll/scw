﻿using System;

namespace ScwSvc.Models
{
    public class Student
    {
        public Guid StudentId { get; set; }

        public Guid SchoolId { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }
    }
}
