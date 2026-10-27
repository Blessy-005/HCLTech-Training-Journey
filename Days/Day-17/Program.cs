using System;
using System.Threading.Tasks;
using AsyncProgrammingPractice;

public class Program
{
    public static async Task Main(string[] args)
    {
        LearnAsyncProgramming practice = new();
        await practice.Run();
    }
}