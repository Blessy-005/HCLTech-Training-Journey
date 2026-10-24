
using System;
using System.Collections.Generic;
using TrainingManagementSystem_ConsoleApp1.Exceptions;
using TrainingManagementSystem_ConsoleApp1.Models;
namespace TrainingManagementSystem_ConsoleApp1.Services
{
    internal class LearnerService
    {
        private List<Learner> learners = new List<Learner>();

        public void RegisterLearner()
        {

            Console.WriteLine("Enter Learner ID:");
            string learnerId = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(learnerId))
            {
                Console.WriteLine("Learner ID cannot be empty. Enter again:");
                learnerId = Console.ReadLine() ?? "";
            }

            bool exists = false;

            foreach (var existingLearner in learners)
            {
                if (existingLearner.LearnerId.Equals(
                    learnerId, StringComparison.OrdinalIgnoreCase))
                {
                    exists = true;
                    break;
                }
            }

            while (exists)
            {
                Console.WriteLine("Learner ID already exists. Enter a different ID:");
                learnerId = Console.ReadLine() ?? "";

                while (string.IsNullOrWhiteSpace(learnerId))
                {
                    Console.WriteLine("Learner ID cannot be empty. Enter again:");
                    learnerId = Console.ReadLine() ?? "";
                }

                exists = false;

                foreach (var existingLearner in learners)
                {
                    if (existingLearner.LearnerId.Equals(
                        learnerId, StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }
            }


            Console.WriteLine("Enter Learner Name:");
            string name = Console.ReadLine() ?? "";

            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Learner name cannot be empty. Enter again:");
                name = Console.ReadLine() ?? "";
            }

            Console.WriteLine("Enter Contact Number:");
            string contactNumber = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Email:");
            string email = Console.ReadLine() ?? "";

            Learner learner = new Learner(
                learnerId, name, contactNumber, email
            );

            learners.Add(learner);

            Console.WriteLine("Learner registered successfully!");
        }

        public void DisplayLearners()
        {
            foreach (var learner in learners)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine($"Learner ID: {learner.LearnerId}");
                Console.WriteLine($"Name: {learner.Name}");
                Console.WriteLine($"Contact: {learner.ContactNumber}");
                Console.WriteLine($"Email: {learner.Email}");
            }
        }

        public Learner? FindLearnerById(string learnerId)
        {
            foreach (Learner learner in learners)
            {
                if (learner.LearnerId.Equals(
                    learnerId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return learner;
                }
            }
            throw new LearnerNotFoundException(
                    "Learner not found with ID: " + learnerId);
        }

    }
}
