/*
 * BT01 - File Creation and Writing
 * This program prompts user to enter a filename and content, then creates the file
 */

namespace BT01;

class Program
{
    static void Main(string[] args)
    {
        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            FILE CREATION AND WRITING TOOL              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            // Get filename from user
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter filename (with .txt extension): ");
            Console.ResetColor();
            string? filename = Console.ReadLine();

            // Validate filename
            if (string.IsNullOrWhiteSpace(filename))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Error: Filename cannot be empty!");
                Console.ResetColor();
                return;
            }

            // Ensure .txt extension
            if (!filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                filename += ".txt";
            }

            // Get content from user
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nEnter content (press Enter twice to finish):");
            Console.ResetColor();

            List<string> contentLines = new List<string>();
            int emptyLineCount = 0;

            while (true)
            {
                string? line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    emptyLineCount++;
                    if (emptyLineCount >= 2)
                        break;
                }
                else
                {
                    emptyLineCount = 0;
                }

                contentLines.Add(line ?? "");
            }

            // Remove the last empty line
            if (contentLines.Count > 0 && string.IsNullOrEmpty(contentLines[contentLines.Count - 1]))
            {
                contentLines.RemoveAt(contentLines.Count - 1);
            }

            string content = string.Join(Environment.NewLine, contentLines);

            // Write content to file
            File.WriteAllText(filename, content);

            // Display success message
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ File created successfully!");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("File Information:");
            Console.ResetColor();
            Console.WriteLine($"  Filename: {filename}");
            Console.WriteLine($"  Full Path: {Path.GetFullPath(filename)}");
            Console.WriteLine($"  File Size: {new FileInfo(filename).Length} bytes");
            Console.WriteLine($"  Lines: {contentLines.Count}");
            Console.WriteLine();

            // Display file content
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("File Content:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(content);
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n⚠ Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
