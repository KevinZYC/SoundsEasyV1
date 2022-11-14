﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundsEasyV1
{
    public class Student
    {
        public int id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string course { get; set; }
        public int grade { get; set; }
        public string email { get; set; }

        public Student(int id, string fname, string lname, string course, int grade, string email)
        {
            this.id = id;
            this.fname = fname;
            this.lname = lname;
            this.course = course;
            this.grade = grade;
            this.email = email;
        }
    }
}