
using System;

namespace TrainingManagementSystem_ConsoleApp1.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public string LearnerId { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public DateTime EnrollmentDate { get; set; }
        public decimal BaseFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalFee { get; set; }
        public EnrollmentStatus Status { get; set; }
    }
}
