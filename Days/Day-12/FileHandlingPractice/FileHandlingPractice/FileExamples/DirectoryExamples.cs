
using System;
using System.IO;

namespace FileHandlingPractice.FileExamples;

public class DirectoryExamples
{
    public void RunExample()
    {
        Console.WriteLine("\n--- Example 3: Directory ---");

        string root = Path.Combine(
            Environment.CurrentDirectory,
            "FileData"
        );

        string reportsPath = Path.Combine(root, "Reports");

        // Create a directory
        Directory.CreateDirectory(reportsPath);

        Console.WriteLine(
            $"Directory exists: {Directory.Exists(reportsPath)}"
        );

        // Create sample report files
        File.WriteAllText(
            Path.Combine(reportsPath, "January.txt"),
            "January report"
        );

        File.WriteAllText(
            Path.Combine(reportsPath, "February.txt"),
            "February report"
        );

        // Get all files
        Console.WriteLine("\nAll files:");

        foreach (string file in Directory.GetFiles(reportsPath))
        {
            Console.WriteLine(file);
        }

        // Get files matching a search pattern
        Console.WriteLine("\nText files:");

        foreach (string file in Directory.GetFiles(reportsPath, "*.txt"))
        {
            Console.WriteLine(Path.GetFileName(file));
        }

        // Create a subdirectory
        Directory.CreateDirectory(
            Path.Combine(reportsPath, "Archive")
        );

        Console.WriteLine("\nSubdirectories:");

        foreach (string directory in Directory.GetDirectories(reportsPath))
        {
            Console.WriteLine(directory);
        }

        // Delete the Reports folder and its contents
        // Uncomment these lines only when you want to delete the sample folder.
        // Directory.Delete(reportsPath, recursive: true);
    }
}
