/*
 * KT3 - Quotient of Two Numbers Calculator
 * This program contains a function to calculate the quotient (division) of two numbers
 */

using System.Text;

namespace KT3;

class Program
{
    static void Main(string[] args)
    {
        // Set console encoding to UTF-8 to support special characters
        Console.OutputEncoding = Encoding.UTF8;

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        QUOTIENT OF TWO NUMBERS CALCULATOR              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            // Get dividend from user
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter dividend (number to be divided): ");
            Console.ResetColor();
            string? input1 = Console.ReadLine();
            double dividend = double.Parse(input1 ?? "0");

            // Get divisor from user
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter divisor (number to divide by): ");
            Console.ResetColor();
            string? input2 = Console.ReadLine();
            double divisor = double.Parse(input2 ?? "0");

            // Check for division by zero
            if (divisor == 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Cannot divide by zero!");
                Console.ResetColor();
            }
            else
            {
                // Calculate quotient using QuotientOfTwoNumbers function
                double result = QuotientOfTwoNumbers(dividend, divisor);

                // Display result
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Result:");
                Console.ResetColor();
                Console.WriteLine("────────────────────────────────────────────────────────");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Quotient of {dividend} ÷ {divisor} = {result}");
                Console.ResetColor();
                Console.WriteLine("────────────────────────────────────────────────────────");
            }
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
    /// Calculates the quotient of two numbers (division)
    /// </summary>
    /// <param name="a">Dividend (number to be divided)</param>
    /// <param name="b">Divisor (number to divide by)</param>
    /// <returns>The quotient of a divided by b</returns>
    static double QuotientOfTwoNumbers(double a, double b)
    {
        return a / b;
    }
}
