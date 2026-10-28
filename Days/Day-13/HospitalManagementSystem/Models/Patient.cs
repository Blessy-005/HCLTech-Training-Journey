using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Models
{
    public class Patient
    {
        public string? PatientId { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public string? PhoneNumber { get; set; }
        public Patient(string patientId,string fullName,int age,string phoneNumber)
        {
            PatientId = patientId;
            FullName = fullName;
            Age = age;
            PhoneNumber = phoneNumber;
        }
    }
}
