using HospitalAppointmentManagement.Utilities;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using System.Collections.Generic;
class Program
{
    static void Main()
    {
        List<Patient> patients = new List<Patient>();
        List<Doctor> doctors = new List<Doctor>();
        List<Appointment> appointments = new List<Appointment>();
        DataPersistence persistence = new DataPersistence();
        try
        {
            persistence.LoadAll(
                out patients,
                out doctors,
                out appointments
            );

            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading data: " + ex.Message);
            return;
        }

        PatientServices patientServices = new PatientServices(patients);
        DoctorServices doctorServices = new DoctorServices(doctors);
        AppointmentServices appointmentServices =
            new AppointmentServices(patients, doctors, appointments);

        //patientServices.RegisterPatient(
        //    new Patient("1", "Blessy", 12, "123456789"));

        //patientServices.RegisterPatient(
        //    new Patient("2", "Catherine", 20, "987654321"));

        //doctorServices.AddDoctor(
        //    new Doctor("100", "Blessy", "Cardiology", 800m));

        //doctorServices.AddDoctor(
        //    new Doctor("101", "Catherine", "Neurology", 1800m));
        

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n===== HOSPITAL MANAGEMENT SYSTEM =====");
            Console.WriteLine("1. Patient Management");
            Console.WriteLine("2. Doctor Management");
            Console.WriteLine("3. Appointment Management");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    PatientMenu(patientServices);
                    break;

                case 2:
                    DoctorMenu(doctorServices);
                    break;

                case 3:
                    AppointmentMenu(
                        appointmentServices, patientServices, doctorServices);
                    break;

                case 4:
                    running = false;
                    Console.WriteLine("Thank you for using the system!");
                    try
                    {
                        persistence.SaveAll(
                            patients,
                            doctors,
                            appointments
                        );

                        Console.WriteLine("All data saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving data: " + ex.Message);
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }


    static void PatientMenu(PatientServices service)
    {
        bool back = false;

        while (!back)
        {
            Console.WriteLine("\n===== PATIENT MANAGEMENT =====");
            Console.WriteLine("1. Register Patient");
            Console.WriteLine("2. Display Patients");
            Console.WriteLine("3. Search Patient");
            Console.WriteLine("4. Update Patient");
            Console.WriteLine("5. Delete Patient");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.Write("Patient ID: ");
                    string id = Console.ReadLine() ?? "";

                    Console.Write("Full Name: ");
                    string name = Console.ReadLine() ?? "";

                    Console.Write("Age: ");
                    if (!int.TryParse(Console.ReadLine(), out int age))
                    {
                        Console.WriteLine("Invalid age.");
                        break;
                    }

                    Console.Write("Phone Number: ");
                    string phone = Console.ReadLine() ?? "";

                    service.RegisterPatient(
                        new Patient(id, name, age, phone));

                    Console.WriteLine("Patient registered.");
                    break;

                case 2:
                    foreach (Patient p in service.DisplayPatients())
                    {
                        Console.WriteLine(
                            $"ID: {p.PatientId} | Name: {p.FullName} | " +
                            $"Age: {p.Age} | Phone: {p.PhoneNumber}");
                    }
                    break;

                case 3:
                    Console.Write("Enter patient name to search: ");
                    string searchName = Console.ReadLine() ?? "";

                    foreach (Patient p in service.SearchByName(searchName))
                    {
                        Console.WriteLine(
                            $"Found: {p.PatientId} - {p.FullName}");
                    }
                    break;

                case 4:
                    Console.Write("Patient ID to update: ");
                    string updateId = Console.ReadLine() ?? "";

                    Console.Write("New Name: ");
                    string newName = Console.ReadLine() ?? "";

                    Console.Write("New Age: ");
                    if (!int.TryParse(Console.ReadLine(), out int newAge))
                    {
                        Console.WriteLine("Invalid age.");
                        break;
                    }

                    Console.Write("New Phone: ");
                    string newPhone = Console.ReadLine() ?? "";

                    bool updated = service.UpdatePatient(
                        updateId, newName, newAge, newPhone);

                    Console.WriteLine(
                        updated ? "Patient updated." : "Patient not found.");
                    break;

                case 5:
                    Console.Write("Patient ID to delete: ");
                    string deleteId = Console.ReadLine() ?? "";

                    bool deleted = service.DeletePatient(deleteId);

                    Console.WriteLine(
                        deleted ? "Patient deleted." : "Patient not found.");
                    break;

                case 6:
                    back = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }


    static void DoctorMenu(DoctorServices service)
    {
        bool back = false;

        while (!back)
        {
            Console.WriteLine("\n===== DOCTOR MANAGEMENT =====");
            Console.WriteLine("1. Add Doctor");
            Console.WriteLine("2. Display Doctors");
            Console.WriteLine("3. Search Doctor");
            Console.WriteLine("4. Update Doctor");
            Console.WriteLine("5. Delete Doctor");
            Console.WriteLine("6. Back to Main Menu");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.Write("Doctor ID: ");
                    string id = Console.ReadLine() ?? "";

                    Console.Write("Full Name: ");
                    string name = Console.ReadLine() ?? "";

                    Console.Write("Specialization: ");
                    string specialization = Console.ReadLine() ?? "";

                    Console.Write("Consultation Fee: ");
                    if (!decimal.TryParse(
                        Console.ReadLine(), out decimal fee))
                    {
                        Console.WriteLine("Invalid fee.");
                        break;
                    }

                    service.AddDoctor(
                        new Doctor(id, name, specialization, fee));

                    Console.WriteLine("Doctor added.");
                    break;

                case 2:
                    foreach (Doctor d in service.DisplayDoctors())
                    {
                        Console.WriteLine(
                            $"ID: {d.DoctorId} | Name: {d.FullName} | " +
                            $"Specialization: {d.Specialization} | " +
                            $"Fee: {d.ConsultationFee}");
                    }
                    break;

                case 3:
                    Console.Write("Doctor name to search: ");
                    string searchName = Console.ReadLine() ?? "";

                    foreach (Doctor d in service.SearchByName(searchName))
                    {
                        Console.WriteLine(
                            $"Found: {d.DoctorId} - {d.FullName}");
                    }
                    break;

                case 4:
                    Console.Write("Doctor ID to update: ");
                    string updateId = Console.ReadLine() ?? "";

                    Console.Write("New Name: ");
                    string newName = Console.ReadLine() ?? "";

                    Console.Write("New Specialization: ");
                    string newSpecialization = Console.ReadLine() ?? "";

                    Console.Write("New Consultation Fee: ");
                    if (!decimal.TryParse(
                        Console.ReadLine(), out decimal newFee))
                    {
                        Console.WriteLine("Invalid fee.");
                        break;
                    }

                    bool updated = service.UpdateDoctor(
                        updateId, newName, newSpecialization, newFee);

                    Console.WriteLine(
                        updated ? "Doctor updated." : "Doctor not found.");
                    break;

                case 5:
                    Console.Write("Doctor name to delete: ");
                    string deleteName = Console.ReadLine() ?? "";

                    bool deleted = service.DeleteDoctor(deleteName);

                    Console.WriteLine(
                        deleted ? "Doctor deleted." : "Doctor not found.");
                    break;

                case 6:
                    back = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }


    static void AppointmentMenu(
        AppointmentServices service,
        PatientServices patientService,
        DoctorServices doctorService)
    {
        bool back = false;

        while (!back)
        {
            Console.WriteLine("\n===== APPOINTMENT MANAGEMENT =====");
            Console.WriteLine("1. Book Appointment");
            Console.WriteLine("2. Display Appointments");
            Console.WriteLine("3. Back to Main Menu");
            Console.Write("Enter your choice: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.Write("Appointment ID: ");
                    string appointmentId = Console.ReadLine() ?? "";

                    Console.Write("Patient ID: ");
                    string patientId = Console.ReadLine() ?? "";

                    Console.Write("Doctor ID: ");
                    string doctorId = Console.ReadLine() ?? "";

                    Console.Write("Reason for appointment: ");
                    string reason = Console.ReadLine() ?? "";

                    Console.Write("Appointment date and time (e.g. 25-10-2026 10:30): ");
                    if (!DateTime.TryParse(
                        Console.ReadLine(), out DateTime appointmentDate))
                    {
                        Console.WriteLine("Invalid date and time.");
                        break;
                    }

                    Appointment appointment = new Appointment(
                        appointmentId,
                        patientId,
                        doctorId,
                        appointmentDate,
                        reason,
                        "Scheduled");

                    service.BookAppointment(appointment);
                    Console.WriteLine("Appointment booking request processed.");
                    break;

                case 2:
                    foreach (Appointment a in service.DisplayAllAppointments())
                    {
                        Console.WriteLine(
                            $"Appointment ID: {a.AppointmentId} | " +
                            $"Reason: {a.Reason} | " +
                            $"Date: {a.AppointmentDateTime} | Status: {a.Status}");
                    }
                    break;

                case 3:
                    back = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}
