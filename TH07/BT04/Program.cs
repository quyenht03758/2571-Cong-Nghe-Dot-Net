/*
 * BT04 - Regular Expression Pattern Matcher
 * This program allows users to test strings against regular expression patterns
 * Features: Custom regex input, default pattern, error handling, and interactive testing
 */

using System.Text.RegularExpressions;

namespace BT04;

class Program
{
    static void Main(string[] args)
    {
        // Default regex pattern: matches one or more digits
        string defaultRegex = @"\d+";

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         REGULAR EXPRESSION PATTERN MATCHER             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Show information about default pattern
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Default Pattern Information:");
        Console.ResetColor();
        Console.WriteLine($"  Pattern: {defaultRegex}");
        Console.WriteLine("  Description: Matches at least one digit");
        Console.WriteLine();

        // Main testing loop
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ResetColor();

            // Get regex pattern from user
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Enter a regular expression (or press ENTER to use default): ");
            Console.ResetColor();
            string? regexInput = Console.ReadLine();

            // Use default regex if user pressed ENTER without input
            string pattern = string.IsNullOrEmpty(regexInput) ? defaultRegex : regexInput;

            // Display the pattern being used
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Using pattern: {pattern}");
            Console.ResetColor();
            Console.WriteLine();

            // Get test string from user
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Enter some input: ");
            Console.ResetColor();
            string? input = Console.ReadLine();

            // Validate input
            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Input cannot be empty.");
                Console.ResetColor();
                continue;
            }

            // Test the pattern against the input
            try
            {
                bool isMatch = Regex.IsMatch(input, pattern);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Result: ");
                Console.ResetColor();

                Console.Write($"\"{input}\" matches \"{pattern}\"? ");

                // Display result with color coding
                if (isMatch)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ True");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("✗ False");
                }
                Console.ResetColor();
            }
            catch (ArgumentException ex)
            {
                // Handle invalid regex pattern
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"⚠ Invalid regular expression: {ex.Message}");
                Console.ResetColor();
            }

            // Prompt for continuation
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press ESC to end or any key to try again.");
            Console.ResetColor();

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Thank you for using the Pattern Matcher!");
                Console.ResetColor();
                break;
            }

            Console.WriteLine();
        }
    }
}
