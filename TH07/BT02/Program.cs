/*
 * BT02 - String Manipulation
 * This program demonstrates string concatenation and various string operations
 */

namespace BT02;

class Program
{
    static void Main(string[] args)
    {
        // Initialize two strings
        string str1 = "Hello";
        string str2 = "World";

        // Display header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║           STRING MANIPULATION OPERATIONS               ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Show original strings
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Original Strings:");
        Console.ResetColor();
        Console.WriteLine($"  String 1: \"{str1}\"");
        Console.WriteLine($"  String 2: \"{str2}\"");
        Console.WriteLine();

        // Step 1: Concatenate two strings
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Step 1: Concatenation");
        Console.ResetColor();
        string concatenated = str1 + " " + str2;
        Console.WriteLine($"  Result: \"{concatenated}\"");
        Console.WriteLine();

        // Step 2: Convert to uppercase
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Step 2: Convert to Uppercase");
        Console.ResetColor();
        string uppercase = concatenated.ToUpper();
        Console.WriteLine($"  Result: \"{uppercase}\"");
        Console.WriteLine();

        // Step 3: Convert to lowercase
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Step 3: Convert to Lowercase");
        Console.ResetColor();
        string lowercase = concatenated.ToLower();
        Console.WriteLine($"  Result: \"{lowercase}\"");
        Console.WriteLine();

        // Step 4: Replace character 'o' with '@'
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Step 4: Replace 'o' with '@'");
        Console.ResetColor();
        string replaced = concatenated.Replace('o', '@');
        Console.WriteLine($"  Result: \"{replaced}\"");
        Console.WriteLine();

        // End program
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
