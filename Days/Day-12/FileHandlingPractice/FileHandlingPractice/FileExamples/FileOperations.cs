
using System;
using System.IO;

namespace FileHandlingPractice.FileExamples;

public class FileOperations
{
    private readonly string _folderPath =
        Path.Combine(Environment.CurrentDirectory, "FileData");

    public void RunExample()
    {
        Console.WriteLine("\n--- Example 1: File Class ---");

        Directory.CreateDirectory(_folderPath);

        string path = Path.Combine(_folderPath, "learner.txt");

        // Write text to a file
        File.WriteAllText(path, "Name: Blessy\nCourse: C#");

        Console.WriteLine("File created.");

        // Check whether the file exists
        Console.WriteLine($"Exists: {File.Exists(path)}");

        // Read all text
        Console.WriteLine("\nFile content:");
        Console.WriteLine(File.ReadAllText(path));

        // Append text
        File.AppendAllText(path, "\nTopic: File Handling");

        Console.WriteLine("\nAfter appending:");
        Console.WriteLine(File.ReadAllText(path));

        // Write multiple lines
        string linesPath = Path.Combine(_folderPath, "learners.txt");

        string[] learners =
        {
            "Arun",
            "Sakthi",
            "Blessy"
        };

        File.WriteAllLines(linesPath, learners);

        // Read all lines
        Console.WriteLine("\nLearner names:");

        foreach (string learner in File.ReadAllLines(linesPath))
        {
            Console.WriteLine(learner);
        }

        // Append multiple lines
        File.AppendAllLines(linesPath, new[] { "David" });

        // Copy a file
        string copyPath = Path.Combine(_folderPath, "learner-copy.txt");
        File.Copy(path, copyPath, overwrite: true);

        Console.WriteLine($"\nFile copied: {File.Exists(copyPath)}");

        // Move or rename a file
        string movedPath = Path.Combine(_folderPath, "renamed-learner.txt");
        File.Move(copyPath, movedPath, overwrite: true);

        Console.WriteLine($"File moved: {File.Exists(movedPath)}");

        // Delete a file
        File.Delete(movedPath);

        Console.WriteLine($"File exists after deletion: {File.Exists(movedPath)}");

        // Create() and Open() are stream-based methods.
        // We will practise them in a separate example if needed.
    }
}
