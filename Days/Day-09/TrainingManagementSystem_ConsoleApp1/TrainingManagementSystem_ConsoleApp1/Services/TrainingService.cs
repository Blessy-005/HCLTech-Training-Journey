using System;
using System.Collections.Generic;
using System.Text;
using TrainingManagementSystem_ConsoleApp1.Models;
using TrainingManagementSystem_ConsoleApp1.Exception;
namespace TrainingManagementSystem_ConsoleApp1.Services
{
    internal class TrainingService
    {
        Course course1 = new Course("C001", "Introduction to Programming", 500.00m, 30, 20);
        Course course2 = new Course("C002", "Web Development Basics", 700.00m, 45, 25);
        Course course3 = new Course("C003", "Data Structures and Algorithms", 800.00m, 60, 15);
        Course course4 = new Course("C004", "Database Management Systems", 600.00m, 40, 20);
        Course course5 = new Course("C005", "Software Engineering Principles", 900.00m, 50, 18);
        Course[] courses;
        public TrainingService()
        {
            courses = new Course[] { course1, course2, course3, course4, course5 };
        }
        public void DisplayCourseDetails() { foreach (var course in courses) { Console.WriteLine($"Course Code: {course.CourseCode}"); Console.WriteLine($"Title: {course.Title}"); Console.WriteLine($"Fee: {course.Fee:C}"); Console.WriteLine($"Duration (Days): {course.DurationDays}"); Console.WriteLine($"Total Seats: {course.TotalSeats}"); Console.WriteLine($"Enrolled Count: {course.EnrolledCount}"); Console.WriteLine(new string('-', 30)); } }
       
public void SearchCourseByCode(string courseCode, string title)
        {
            bool found = false;

            foreach (var course in courses)
            {
                if ((!string.IsNullOrWhiteSpace(courseCode) &&
                     course.CourseCode.Equals(
                         courseCode, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(title) &&
                     course.Title.Contains(
                         title, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Course found!");
                    Console.WriteLine($"Course Code: {course.CourseCode}");
                    Console.WriteLine($"Title: {course.Title}");
                    Console.WriteLine($"Fee: ₹{course.Fee:F2}");
                    Console.WriteLine($"Duration: {course.DurationDays} days");
                    Console.WriteLine($"Total Seats: {course.TotalSeats}");
                    Console.WriteLine($"Enrolled Count: {course.EnrolledCount}");

                    found = true;
                }
            }

            if (!found)
            {
                throw new CourseNotFoundException(
                    "Course not found with the provided code or title.");
            }
        }
        public void SortCoursesByFee(bool ascending) { if (ascending) Array.Sort(courses, (a, b) => a.Fee.CompareTo(b.Fee)); else Array.Sort(courses, (a, b) => b.Fee.CompareTo(a.Fee)); foreach (var course in courses) { Console.WriteLine("---------------------------"); Console.WriteLine($"{course.Title} - ₹{course.Fee:F2}"); Console.WriteLine("---------------------------"); } }
        public void AvailableSeats() { foreach (var course in courses) { int availableSeats = course.TotalSeats - course.EnrolledCount; Console.WriteLine($"Course Code: {course.CourseCode}"); Console.WriteLine($"Title: {course.Title}"); Console.WriteLine($"Available Seats: {availableSeats}"); Console.WriteLine(new string('-', 30)); } }


        public Course? FindCourseByCode(string courseCode)
        {
            foreach (Course course in courses)
            {
                if (course.CourseCode.Equals(
                    courseCode,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return course;
                }
            }

            return null;
        }

    }
}
