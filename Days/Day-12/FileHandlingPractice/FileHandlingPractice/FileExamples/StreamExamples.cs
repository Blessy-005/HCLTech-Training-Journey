
using System;
using System.IO;

namespace FileHandlingPractice.FileExamples;

public class StreamExamples
{
    public void RunExample()
    {
        Console.WriteLine("\n--- Example 4: Streams ---");

        string folder = Path.Combine(
            Environment.CurrentDirectory,
            "FileData"
        );

        Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, "stream-notes.txt");

        // StreamWriter writes text
        using (StreamWriter writer = new(path))
        {
            writer.WriteLine("Learning C#");
            writer.WriteLine("Learning File Handling");
            writer.WriteLine("Learning Streams");
        }

        Console.WriteLine("Text written using StreamWriter.");

        // StreamReader reads text one line at a time
        Console.WriteLine("\nReading using StreamReader:");

        using (StreamReader reader = new(path))
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }

        // FileStream writes raw bytes
        string binaryPath = Path.Combine(folder, "sample.bin");

        byte[] data = { 65, 66, 67 };

        using (FileStream stream = new(
            binaryPath,
            FileMode.Create,
            FileAccess.Write))
        {
            stream.Write(data, 0, data.Length);
        }

        Console.WriteLine("\nBytes written using FileStream.");

        using (FileStream stream = new(
            binaryPath,
            FileMode.Open,
            FileAccess.Read))
        {
            byte[] buffer = new byte[stream.Length];

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            Console.WriteLine(
                $"Bytes read: {bytesRead}"
            );

            Console.WriteLine(
                $"Text from bytes: {System.Text.Encoding.ASCII.GetString(buffer)}"
            );
        }
    }
}
