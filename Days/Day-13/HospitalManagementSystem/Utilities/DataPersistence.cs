
using HospitalManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HospitalAppointmentManagement.Utilities
{
    public class DataPersistence
    {
        private readonly string _folderPath =
            AppDomain.CurrentDomain.BaseDirectory;

        private readonly JsonSerializerOptions _options =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        // Save any list to a JSON file
        public void SaveData<T>(List<T> data, string fileName)
        {
            string path = Path.Combine(_folderPath, fileName);
            string tempPath = path + ".tmp";

            string json = JsonSerializer.Serialize(data, _options);

            try
            {
                File.WriteAllText(tempPath, json);

                File.Move(tempPath, path, true);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        public List<T> LoadData<T>(string fileName)
        {
            string path = Path.Combine(_folderPath, fileName);

            if (!File.Exists(path))
            {
                return new List<T>();
            }

            try
            {
                string json = File.ReadAllText(path);

                return JsonSerializer.Deserialize<List<T>>(json)
                       ?? throw new JsonException(
                           "The JSON file contains null instead of a list.");
            }
            catch (JsonException ex)
            {
                throw new DataFileException(
                    $"Could not load {fileName}: invalid JSON. " +
                    "The original file has not been overwritten.",
                    ex);
            }
        }

        public void SaveAll(
            List<Patient> patients,
            List<Doctor> doctors,
            List<Appointment> appointments)
        {
            SaveData(patients, "patients.json");
            SaveData(doctors, "doctors.json");
            SaveData(appointments, "appointments.json");
        }

        public void LoadAll(
            out List<Patient> patients,
            out List<Doctor> doctors,
            out List<Appointment> appointments)
        {
            patients = LoadData<Patient>("patients.json");
            doctors = LoadData<Doctor>("doctors.json");
            appointments = LoadData<Appointment>("appointments.json");
        }
    }

    public class DataFileException : Exception
    {
        public DataFileException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
