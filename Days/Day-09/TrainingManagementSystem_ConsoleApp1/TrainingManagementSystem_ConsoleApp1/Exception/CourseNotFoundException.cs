using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingManagementSystem_ConsoleApp1.Exception
{
    internal class CourseNotFoundException:System.Exception
    {
        public CourseNotFoundException(string msg) : base(msg)
        {

        }
    }
}
