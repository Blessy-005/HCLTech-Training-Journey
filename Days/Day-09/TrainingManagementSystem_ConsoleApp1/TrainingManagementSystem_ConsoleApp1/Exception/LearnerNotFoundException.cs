
using System;

namespace TrainingManagementSystem_ConsoleApp1.Exceptions
{
    internal class LearnerNotFoundException : System.Exception
    {
        public LearnerNotFoundException(string message)
            : base(message)
        {
        }
    }
}
