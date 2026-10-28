using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Models
{
    public class Doctor
    {
        public string? DoctorId { get; set; }
        public string? FullName { get; set; }
        public string? Specialization { get; set; }
        public decimal ConsultationFee { get; set; }
        public Doctor(string doctorId,string fullName,string specialization,decimal consultationFee)
        {
            DoctorId = doctorId;
            FullName = fullName;
            Specialization = specialization;
            ConsultationFee = consultationFee;
        }
    }
}
