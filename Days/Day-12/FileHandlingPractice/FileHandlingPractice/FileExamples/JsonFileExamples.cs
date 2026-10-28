
using System;
using System.IO;
using System.Text.Json;

namespace FileHandlingPractice.FileExamples;

public class JsonFileExamples
{
    private class Learner
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Course { get; set; } = "";
    }

    public void RunExample()
    {
        Console.WriteLine("\n--- Example 5: JSON File Handling ---");

        string folder = Path.Combine(
            Environment.CurrentDirectory,
            "FileData"
        );

        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, "learner.json");

        // Create an object
        Learner learner = new()
        {
            Id = 101,
            Name = "Sakthi",
            Course = "Java"
        };

        // Convert the object to JSON
        string json = JsonSerializer.Serialize(
            learner,
            new JsonSerializerOptions { WriteIndented = true }
        );

        // Save JSON to a file
        File.WriteAllText(path, json);

        Console.WriteLine("Learner saved to JSON.");

        Console.WriteLine("\nJSON content:");
        Console.WriteLine(File.ReadAllText(path));

        // Read JSON from the file
        string savedJson = File.ReadAllText(path);

        // Convert JSON back into an object
        Learner? loadedLearner =
            JsonSerializer.Deserialize<Learner>(savedJson);

        if (loadedLearner != null)
        {
            Console.WriteLine("\nDeserialized learner:");
            Console.WriteLine($"ID: {loadedLearner.Id}");
            Console.WriteLine($"Name: {loadedLearner.Name}");
            Console.WriteLine($"Course: {loadedLearner.Course}");
        }
    }
}
