
using FileHandlingPractice.FileExamples;

FileOperations fileOperations = new();
FileInfoExamples fileInfoExamples = new();
DirectoryExamples directoryExamples = new();
StreamExamples streamExamples = new();
JsonFileExamples jsonFileExamples = new();

fileOperations.RunExample();
fileInfoExamples.RunExample();
directoryExamples.RunExample();
streamExamples.RunExample();
jsonFileExamples.RunExample();

Console.WriteLine("\nAll file handling examples completed!");
