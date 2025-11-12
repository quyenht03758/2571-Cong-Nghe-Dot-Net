/*
 * KT2 - Product of Two Numbers Calculator
 * This program contains a function to calculate the product of two numbers
 */

using System.Text;

namespace KT2;

class Program
{
    static void Main(string[] args)
    {
        // Set console encoding to UTF-8 to support special characters
        Console.OutputEncoding = Encoding.UTF8;

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         PRODUCT OF TWO NUMBERS CALCULATOR              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            // Get first number from user
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter first number: ");
            Console.ResetColor();
            string? input1 = Console.ReadLine();
            double number1 = double.Parse(input1 ?? "0");

            // Get second number from user
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter second number: ");
            Console.ResetColor();
            string? input2 = Console.ReadLine();
            double number2 = double.Parse(input2 ?? "0");

            // Calculate product using ProductOfTwoNumbers function
            double result = ProductOfTwoNumbers(number1, number2);

            // Display result
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Result:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Product of {number1} and {number2} = {result}");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
        }
        catch (FormatException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nError: Invalid number format!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {ex.Message}");
            Console.ResetColor();
        }

        // End program
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }

    /// <summary>
    /// Calculates the product of two numbers
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>The product of a and b</returns>
    static double ProductOfTwoNumbers(double a, double b)
    {
        return a * b;
    }
}
