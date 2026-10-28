using HospitalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagementSystem.Services
{
    public class AppointmentServices
    {
        private readonly List<Patient> _patients;
        private readonly List<Doctor> _doctors;
        private readonly List<Appointment> _appointments;
        public AppointmentServices(List<Patient> p, List<Doctor> d, List<Appointment> a)
        {
            _patients = p;
            _doctors = d;
            _appointments = a;
        }
        public void BookAppointment(Appointment appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.AppointmentId))
            {
                throw new ArgumentException("Appointment Id is required..");
            }
            if (_appointments.Any(a => a.AppointmentId.Equals(appointment.AppointmentId, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Appointment  already exist!");
            }
            if (appointment.AppointmentDateTime <= DateTime.Now)
            {
                throw new ArgumentException("Appointment mut be scheduled in future.");
            }
            Patient? patient = _patients.FirstOrDefault(p =>
                p.PatientId.Equals(
                    appointment.PatientId,
                    StringComparison.OrdinalIgnoreCase));

            if (patient == null)
                throw new ArgumentException("Patient does not exist.");

            Doctor? doctor = _doctors.FirstOrDefault(d =>
                d.DoctorId.Equals(
                    appointment.DoctorId,
                    StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
                throw new ArgumentException("Doctor does not exist.");



            bool slotTaken = _appointments.Any(a =>
                a.DoctorId.Equals(
                    appointment.DoctorId,
                    StringComparison.OrdinalIgnoreCase)
                && a.AppointmentDateTime == appointment.AppointmentDateTime
                && a.Status == "Scheduled");

            if (slotTaken)
                throw new InvalidOperationException(
                    "Doctor already has an appointment at this time.");

            appointment.Status = "Scheduled";
            _appointments.Add(appointment);
        }

        public  List<Appointment> DisplayAllAppointments()
        {
            return _appointments.ToList();
        }

        public Appointment? GetAppointmentById(string appointmentId)
        {
            return _appointments.FirstOrDefault(a =>
                a.AppointmentId.Equals(
                    appointmentId,
                    StringComparison.OrdinalIgnoreCase));
        }


        public bool RescheduleAppointment(
            string appointmentId,
            DateTime newDateTime)
        {
            Appointment? appointment = GetAppointmentById(appointmentId);

            if (appointment == null)
                return false;

            if (appointment.Status != "Scheduled")
                throw new InvalidOperationException(
                    "Only scheduled appointments can be rescheduled.");

            if (newDateTime <= DateTime.Now)
                throw new ArgumentException(
                    "New appointments  must be in the future.");

            bool slotTaken = _appointments.Any(a =>
                a.AppointmentId != appointmentId
                && a.DoctorId.Equals(
                    appointment.DoctorId,
                    StringComparison.OrdinalIgnoreCase)
                && a.AppointmentDateTime == newDateTime
                && a.Status == "Scheduled");

            if (slotTaken)
                throw new InvalidOperationException(
                    "Doctor already has an appointment at this time.");

            appointment.AppointmentDateTime = newDateTime;
            return true;
        }

        public bool CancelAppointment(string appointmentId)
        {
            Appointment? appointment = GetAppointmentById(appointmentId);
            if (appointment == null)
            {
                return false;
            }
            appointment.Status = "Cancelled";
            return true;
        }
        public bool CompleteAppointment(string appointmentId)
        {
            Appointment? appointment = GetAppointmentById(appointmentId);
            if (appointment == null)
            {
                return false;
            }
            appointment.Status = "Completed";
            return true;
        }

    }

    }

