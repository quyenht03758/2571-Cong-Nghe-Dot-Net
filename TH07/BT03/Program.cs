/*
 * BT03 - String Splitting with Quoted Values
 * This program splits quoted movie titles that may contain commas
 * Algorithm: Parse string respecting double quotes as delimiters
 */

using System.Text.RegularExpressions;

namespace BT03;

class Program
{
    static void Main(string[] args)
    {
        // Input string with quoted movie titles
        // Each title is enclosed in double quotes and separated by commas
        string movies = "\"Monsters, Inc.\",\"I, Tonya\",\"Lock, Stock and Two Smoking Barrels\"";

        // Display header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            SMART STRING SPLITTING PROGRAM              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Show original string
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Original String:");
        Console.ResetColor();
        Console.WriteLine($"  {movies}");
        Console.WriteLine();

        // Parse the quoted strings using regex
        // Pattern matches text within double quotes
        List<string> titles = new List<string>();

        // Regex pattern: matches content within double quotes
        // \"([^\"]*)\": captures everything between quotes
        MatchCollection matches = Regex.Matches(movies, "\"([^\"]*)\"");

        foreach (Match match in matches)
        {
            // Group 1 contains the text between quotes (without the quotes)
            titles.Add(match.Groups[1].Value);
        }

        // Display parsed movie titles
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Parsed Movie Titles:");
        Console.ResetColor();
        Console.WriteLine("────────────────────────────────────────────────────────");

        int count = 1;
        foreach (string title in titles)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {count}. ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(title);
            Console.ResetColor();
            count++;
        }

        // End program
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
