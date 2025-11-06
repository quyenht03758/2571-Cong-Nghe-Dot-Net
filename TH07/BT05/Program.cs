/*
 * BT05 - Number to Words Converter
 * This program converts numeric input (up to 21 digits) into English words
 * Algorithm: Recursive decomposition using place values (quintillions down to units)
 */

namespace BT05;

class Program
{
    static void Main(string[] args)
    {
        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            NUMBER TO WORDS CONVERTER                   ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Instructions:");
        Console.ResetColor();
        Console.WriteLine("  • Enter a number up to 21 digits");
        Console.WriteLine("  • The program will convert it to English words");
        Console.WriteLine("  • Press Ctrl+C to exit");
        Console.WriteLine();

        // Main conversion loop
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ResetColor();

            // Get user input
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Enter a number up to twenty one digits long: ");
            Console.ResetColor();
            string? input = Console.ReadLine();

            // Validate: Check if input is empty
            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Input cannot be empty.");
                Console.ResetColor();
                Console.WriteLine();
                continue;
            }

            // Remove leading zeros for proper parsing
            input = input.TrimStart('0');

            // Special case: Input is zero
            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n0 in words is zero.");
                Console.ResetColor();
                Console.WriteLine();
                continue;
            }

            // Validate: Check if input is a valid number
            if (!long.TryParse(input, out long number))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Invalid number. Please enter a valid number up to 21 digits.");
                Console.ResetColor();
                Console.WriteLine();
                continue;
            }

            // Validate: Check if number is within supported range
            if (input.Length > 21)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Number is too large. Please enter a number up to 21 digits.");
                Console.ResetColor();
                Console.WriteLine();
                continue;
            }

            // Convert number to formatted string and words
            string formatted = FormatNumber(number);
            string words = NumberToWords(number);

            // Display result
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Numeric: ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(formatted);
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("In Words: ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(words);
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Formats a number with thousand separators (e.g., 1,234,567)
    /// </summary>
    /// <param name="number">The number to format</param>
    /// <returns>Formatted string with thousand separators</returns>
    static string FormatNumber(long number)
    {
        return number.ToString("N0");
    }

    /// <summary>
    /// Recursively converts a number to its English word representation
    /// Supports numbers up to quintillions (10^18)
    /// </summary>
    /// <param name="number">The number to convert</param>
    /// <returns>English word representation of the number</returns>
    static string NumberToWords(long number)
    {
        // Base case: zero
        if (number == 0)
            return "zero";

        // Handle negative numbers
        if (number < 0)
            return "minus " + NumberToWords(Math.Abs(number));

        string words = "";

        // Process quintillions (10^18)
        if ((number / 1000000000000000000) > 0)
        {
            words += NumberToWords(number / 1000000000000000000) + " quintillion ";
            number %= 1000000000000000000;
        }

        // Process quadrillions (10^15)
        if ((number / 1000000000000000) > 0)
        {
            words += NumberToWords(number / 1000000000000000) + " quadrillion ";
            number %= 1000000000000000;
        }

        // Process trillions (10^12)
        if ((number / 1000000000000) > 0)
        {
            words += NumberToWords(number / 1000000000000) + " trillion ";
            number %= 1000000000000;
        }

        // Process billions (10^9)
        if ((number / 1000000000) > 0)
        {
            words += NumberToWords(number / 1000000000) + " billion ";
            number %= 1000000000;
        }

        // Process millions (10^6)
        if ((number / 1000000) > 0)
        {
            words += NumberToWords(number / 1000000) + " million ";
            number %= 1000000;
        }

        // Process thousands (10^3)
        if ((number / 1000) > 0)
        {
            words += NumberToWords(number / 1000) + " thousand ";
            number %= 1000;
        }

        // Process hundreds (10^2)
        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " hundred ";
            number %= 100;
        }

        // Process remaining numbers (0-99)
        if (number > 0)
        {
            // Add "and" if there are previous words
            if (words != "")
                words += "and ";

            // Lookup arrays for units and tens
            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            // Handle numbers 0-19 (special cases)
            if (number < 20)
            {
                words += unitsMap[number];
            }
            else
            {
                // Handle numbers 20-99
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
        }

        return words.Trim();
    }
}
