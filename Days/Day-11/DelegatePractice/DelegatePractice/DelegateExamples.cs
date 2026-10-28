
using System;
using System.Linq;

namespace DelegatePractice;

public class DelegateExamples
{
    // Example 1: Custom Delegate

    public delegate void DelegateName(string message);

    public void Display(string msg)
    {
        Console.WriteLine(msg);
    }

    public void RunExample1()
    {
        Console.WriteLine("\n--- Example 1: Custom Delegate ---");

        DelegateName a = Display;
        a("Hello");
    }


    // Example 2: Action<T>

    public void RunExample2()
    {
        Console.WriteLine("\n--- Example 2: Action ---");

        Action<string> m = Display;
        m("Hello all");
    }


    // Example 3: Func<T, TResult>

    public int Check(string s, char ch)
    {
        return s.IndexOf(ch);
    }

    public void RunExample3()
    {
        Console.WriteLine("\n--- Example 3: Func ---");

        Func<string, char, int> m = Check;

        Console.WriteLine(m("world", 'r'));
        Console.WriteLine(m("hello", 'z'));
    }


    // Example 4: Predicate<T>

    public bool IsPalindrome(string s)
    {
        return s == new string(s.Reverse().ToArray());
    }

    public void RunExample4()
    {
        Console.WriteLine("\n--- Example 4: Predicate ---");

        Predicate<string> m = IsPalindrome;

        Console.WriteLine(m("world"));
        Console.WriteLine(m("helleh"));
    }


    // Example 5: Lambda Expression

    public void RunExample5()
    {
        Console.WriteLine("\n--- Example 5: Lambda Expression ---");

        Predicate<string> m =
            msg => msg == new string(msg.Reverse().ToArray());

        Console.WriteLine(m("world"));
        Console.WriteLine(m("helleh"));
    }


    // Example 6: Passing Predicate as a Parameter

    public void CheckString(string s, Predicate<string> condition)
    {
        Console.WriteLine(condition(s));
    }

    public void RunExample6()
    {
        Console.WriteLine("\n--- Example 6: Predicate as Parameter ---");

        CheckString("malayalam", IsPalindrome);

        CheckString(
            "Hello",
            s => s.Distinct().Count() == s.Length
        );

        CheckString("programming", s => s.Length > 8);

        CheckString("level", s => s[0] == s[s.Length - 1]);
    }


    // Example 7: Generic Method with Predicate<T>

    public void CheckValue<T>(T value, Predicate<T> condition)
    {
        Console.WriteLine(condition(value));
    }

    public void RunExample7()
    {
        Console.WriteLine("\n--- Example 7: Generic Predicate ---");

        CheckValue<string>(
            "abcba",
            s => s == new string(s.Reverse().ToArray())
        );

        CheckValue<string>(
            "level",
            s => s[0] == s[s.Length - 1]
        );

        CheckValue<int[]>(
            new int[] { 1, 2, 4, 5 },
            arr => arr[0] == arr[arr.Length - 1]
        );
    }


    // Example 8: Generic Method with Func<T, TResult>

    public void CheckValue<T, TResult>(
        T value,
        Func<T, TResult> condition)
    {
        Console.WriteLine(condition(value));
    }

    public void RunExample8()
    {
        Console.WriteLine("\n--- Example 8: Generic Func ---");

        CheckValue<string, bool>(
            "abcba",
            s => s == new string(s.Reverse().ToArray())
        );

        CheckValue<string, string>(
            "level",
            s => s[0] + "" + s[s.Length - 1]
        );

        CheckValue<int[], int>(
            new int[] { 1, 2, 4, 5 },
            arr => arr[0] + arr[arr.Length - 1]
        );
    }
}
