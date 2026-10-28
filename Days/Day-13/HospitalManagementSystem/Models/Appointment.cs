using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Models
{
    public class Appointment
    {
        public string? AppointmentId { get; set; }
        public string? PatientId { get; set; }
        public string? DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public Appointment(string appointmentId, string patientId, string doctorId, DateTime AppointmentDateTime, string reason, string status)
        {
            AppointmentId = appointmentId;
            PatientId = patientId;
            DoctorId = doctorId;
            AppointmentId = appointmentId;
            Reason = reason;
            Status = status;
        }

    }
}
