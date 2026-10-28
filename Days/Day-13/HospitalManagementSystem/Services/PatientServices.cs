using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Services
{
    public class PatientServices
    {
        private readonly List<Patient> _patients;
        public PatientServices(List<Patient> patients)
        {
            _patients = patients;
        }
        public void RegisterPatient(Patient patient)
        {
            if (string.IsNullOrWhiteSpace(patient.PatientId))
            {
                throw new ArgumentException("Patient Id is required. ");
            }
            if (string.IsNullOrWhiteSpace(patient.FullName))
            {
                throw new ArgumentException("Name is required.");
            }
            if(patient.Age <= 0)
            {
                throw new ArgumentException("Invalid age.");
            }
            if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            bool Exist = _patients.Any(p => p.PatientId.Equals(patient.PatientId, StringComparison.OrdinalIgnoreCase));
            if (Exist)
            {
                throw new ArgumentException("Patient already exist.");
            }

            _patients.Add(patient);

        }
        public List<Patient> DisplayPatients()
        {
            return _patients.ToList();

        }
        public Patient? SearchPatientById(string patientId)
        {
            return _patients.FirstOrDefault(
                p => p.PatientId.Equals(patientId, StringComparison.OrdinalIgnoreCase));
        }
        public List<Patient> SearchByName(string name)
        {
            return _patients.Where(
                p=> p.FullName.Contains(name,StringComparison.OrdinalIgnoreCase)
                ).ToList();
        }
        public bool UpdatePatient(string patientId,string fullName,int age,string phoneNumber)
        {
            Patient? patient = SearchPatientById(patientId);
            if (patient == null)
            {
                return false;
            }

          
            if (string.IsNullOrWhiteSpace(patient.FullName))
            {
                throw new ArgumentException("Name is required.");
            }
            if (patient.Age <= 0)
            {
                throw new ArgumentException("Invalid age.");
            }
            if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }
            patient.FullName = fullName;
            patient.Age = age;
            patient.PhoneNumber = phoneNumber;
            return true;
        }
        public bool DeletePatient(string patientID)
        {
            Patient? patient = SearchPatientById(patientID);
            if (patient == null)
            {
                return false;
            }
            _patients.Remove(patient);
            return true;
        }
    }
}
