
using System;
using System.IO;

namespace FileHandlingPractice.FileExamples;

public class FileInfoExamples
{
    public void RunExample()
    {
        Console.WriteLine("\n--- Example 2: FileInfo ---");

        string path = Path.Combine(
            Environment.CurrentDirectory,
            "FileData",
            "learner.txt"
        );

        FileInfo file = new(path);

        if (file.Exists)
        {
            Console.WriteLine($"Name: {file.Name}");
            Console.WriteLine($"Full path: {file.FullName}");
            Console.WriteLine($"Extension: {file.Extension}");
            Console.WriteLine($"Size: {file.Length} bytes");
            Console.WriteLine($"Created: {file.CreationTime}");
            Console.WriteLine($"Last modified: {file.LastWriteTime}");
            Console.WriteLine($"Read only: {file.IsReadOnly}");
            Console.WriteLine($"Directory: {file.DirectoryName}");

            // Refresh cached file information
            file.Refresh();
        }
        else
        {
            Console.WriteLine("File not found.");
        }
    }
}
