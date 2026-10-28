using HospitalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HospitalManagementSystem.Services
{
    public class DoctorServices
    {
        private readonly List<Doctor> _doctors;
        public DoctorServices(List<Doctor> doctors)
        {
            _doctors = doctors;
        }
        public void AddDoctor(Doctor doctor)
        {
            if (string.IsNullOrWhiteSpace(doctor.DoctorId))
            {
                throw new ArgumentException("Doctor Id is required. ");
            }
            if (string.IsNullOrWhiteSpace(doctor.FullName))
            {
                throw new ArgumentException("Full Name is required.");
            }
            
            if (string.IsNullOrWhiteSpace(doctor.Specialization))
            {
                throw new ArgumentException("Specialization is required.");
            }
            if (doctor.ConsultationFee <= 0)
            {
                throw new ArgumentException("Invalid Fee.");
            }

            bool Exist = _doctors.Any(p => p.DoctorId.Equals(doctor.DoctorId, StringComparison.OrdinalIgnoreCase));
            if (Exist)
            {
                throw new ArgumentException("Doctor already exist.");
            }

            _doctors.Add(doctor);

        }
        public List<Doctor> DisplayDoctors()
        {
            return _doctors.ToList();

        }
        public Doctor? SearchDoctorById(string Searchtext)
        {
            return _doctors.FirstOrDefault(
                p => p.FullName.Equals(Searchtext, StringComparison.OrdinalIgnoreCase) || p.Specialization.Contains(Searchtext,StringComparison.OrdinalIgnoreCase));
        }
        public List<Doctor> SearchByName(string name)
        {
            return _doctors.Where(
                p => p.FullName.Contains(name, StringComparison.OrdinalIgnoreCase)
                ).ToList();
        }
        public bool UpdateDoctor(string DoctorId, string fullName, string specialization,decimal fee)
        {
            Doctor? doctor = SearchDoctorById(fullName);
            if (doctor == null)
            {
                return false;
            }


            if (string.IsNullOrWhiteSpace(doctor.FullName))
            {
                throw new ArgumentException("Name is required.");
            }
            
            if (string.IsNullOrWhiteSpace(doctor.Specialization))
            {
                throw new ArgumentException("Specialization is required.");
            }
            if (doctor.ConsultationFee <= 0)
            {
                throw new ArgumentException("Invalid fee.");
            }
            doctor.FullName = fullName;
            doctor.Specialization = specialization;
            doctor.ConsultationFee = fee;
          
            return true;
        }
        public bool DeleteDoctor(string name)
        {
            Doctor? doctor = SearchDoctorById(name);
            if (doctor == null)
            {
                return false;
            }
            _doctors.Remove(doctor);
            return true;
        }
    }
}
