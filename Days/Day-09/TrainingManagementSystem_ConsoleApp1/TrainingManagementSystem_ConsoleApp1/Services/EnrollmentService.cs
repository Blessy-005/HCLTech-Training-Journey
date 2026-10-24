
using System;
using System.Collections.Generic;
using TrainingManagementSystem_ConsoleApp1.Exceptions;
using TrainingManagementSystem_ConsoleApp1.Models;

namespace TrainingManagementSystem_ConsoleApp1.Services
{
    internal class EnrollmentService
    {
        private List<Enrollment> enrollments = new List<Enrollment>();

        private TrainingService trainingService;
        private LearnerService learnerService;

        public EnrollmentService(
            TrainingService trainingService,
            LearnerService learnerService)
        {
            this.trainingService = trainingService;
            this.learnerService = learnerService;
        }

        public void CheckLearnerAndCourse()
        {
            Console.Write("Enter Learner ID: ");
            string learnerId = Console.ReadLine() ?? "";

            Learner? learner = learnerService.FindLearnerById(learnerId);

            if (learner == null)
            {
                Console.WriteLine("Learner not found!");
                return;
            }

            Console.Write("Enter Course Code: ");
            string courseCode = Console.ReadLine() ?? "";

            Course? course = trainingService.FindCourseByCode(courseCode);

            if (course == null)
            {
                Console.WriteLine("Course not found!");
                return;
            }


            Console.WriteLine("Learner found: " + learner.Name);
            Console.WriteLine("Course found: " + course.Title);

            int availableSeats = course.TotalSeats - course.EnrolledCount;

            Console.WriteLine("Available Seats: " + availableSeats);

            if (availableSeats <= 0)
            {
                Console.WriteLine("Sorry, this course is full!");
                return;
            }

            Console.WriteLine("Seats are available. You can proceed with enrollment.");

        }

        private bool IsAlreadyEnrolled(string learnerId, string courseCode)
        {
            foreach (Enrollment enrollment in enrollments)
            {
                if (enrollment.LearnerId.Equals(
                        learnerId, StringComparison.OrdinalIgnoreCase)
                    && enrollment.CourseCode.Equals(
                        courseCode, StringComparison.OrdinalIgnoreCase)
                    && enrollment.Status == EnrollmentStatus.Active)
                {
                    return true;
                }
            }

            return false;
        }

        public void EnrollLearner()
        {
            Console.Write("Enter Learner ID: ");
            string learnerId = Console.ReadLine() ?? "";


            Learner? learner =null;

            try
            {
                learner = learnerService.FindLearnerById(learnerId);
            }
            catch (LearnerNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }


            Console.Write("Enter Course Code: ");
            string courseCode = Console.ReadLine() ?? "";

            Course? course = trainingService.FindCourseByCode(courseCode);

            if (course == null)
            {
                Console.WriteLine("Course not found!");
                return;
            }

            if (IsAlreadyEnrolled(learnerId, courseCode))
            {
                Console.WriteLine("Learner is already enrolled in this course!");
                return;
            }

            int availableSeats = course.TotalSeats - course.EnrolledCount;

            if (availableSeats <= 0)
            {
                Console.WriteLine("Course is full!");
                return;
            }



            decimal discount = 0;

            if (course.Fee >= 5000)
            {
                discount = course.Fee * 10 / 100;
            }

            decimal finalFee = course.Fee - discount;

            Enrollment enrollment = new Enrollment
            {
                EnrollmentId = enrollments.Count + 1,
                LearnerId = learnerId,
                CourseCode = courseCode,
                EnrollmentDate = DateTime.Now,
                BaseFee = course.Fee,
                DiscountAmount = discount,
                FinalFee = finalFee,
                Status = EnrollmentStatus.Active
            };


            enrollments.Add(enrollment);
            course.EnrolledCount++;

            Console.WriteLine("Enrollment successful!");
            Console.WriteLine("Enrollment ID: " + enrollment.EnrollmentId);
            Console.WriteLine("Learner: " + learner.Name);
            Console.WriteLine("Course: " + course.Title);
            Console.WriteLine("Base Fee: " + enrollment.BaseFee);
            Console.WriteLine("Discount: " + enrollment.DiscountAmount);
            Console.WriteLine("Final Fee: " + enrollment.FinalFee); Console.WriteLine("Available seats: " +
                (course.TotalSeats - course.EnrolledCount));
        }

        public void DisplayEnrollments()
        {
            if (enrollments.Count == 0)
            {
                Console.WriteLine("No enrollments found.");
                return;
            }

            foreach (Enrollment enrollment in enrollments)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("Enrollment ID: " + enrollment.EnrollmentId);
                Console.WriteLine("Learner ID: " + enrollment.LearnerId);
                Console.WriteLine("Course Code: " + enrollment.CourseCode);
                Console.WriteLine("Enrollment Date: " + enrollment.EnrollmentDate);
                Console.WriteLine("Base Fee: " + enrollment.BaseFee);
                Console.WriteLine("Discount: " + enrollment.DiscountAmount);
                Console.WriteLine("Final Fee: " + enrollment.FinalFee);
                Console.WriteLine("Status: " + enrollment.Status);
            }
        }

        public void CancelEnrollment(int enrollmentId)
        {
            Enrollment? enrollment = null;

            foreach (Enrollment item in enrollments)
            {
                if (item.EnrollmentId == enrollmentId)
                {
                    enrollment = item;
                    break;
                }
            }

            if (enrollment == null)
            {
                Console.WriteLine("Enrollment not found.");
                return;
            }

            if (enrollment.Status == EnrollmentStatus.Cancelled)
            {
                Console.WriteLine("Enrollment is already cancelled.");
                return;
            }

            Course? course = trainingService.FindCourseByCode(
                enrollment.CourseCode);

            if (course == null)
            {
                Console.WriteLine("Course not found.");
                return;
            }

            enrollment.Status = EnrollmentStatus.Cancelled;
            course.EnrolledCount--;

            Console.WriteLine("Enrollment cancelled successfully.");
            Console.WriteLine("Enrollment ID: " + enrollment.EnrollmentId);
            Console.WriteLine("Course: " + course.Title);
            Console.WriteLine("Available seats: " +
                (course.TotalSeats - course.EnrolledCount));
        }


    }
}
