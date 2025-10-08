namespace BT03
{
    internal class Program
    {
        static int ReadNonNegative(string label, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write($"{label} ");
                if (int.TryParse(Console.ReadLine(), out int v) && v >= 0 && v <= max)
                    return v;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid input. Please enter a non-negative integer within range.\n");
                Console.ResetColor();
            }
        }

        static void Main()
        {
            Console.Title = "BT03 - Convert to Total Seconds";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===== BT03: CONVERT HOUR–MINUTE–SECOND TO TOTAL SECONDS =====\n");
            Console.ResetColor();

            int h = ReadNonNegative("Enter hours   (>= 0):");
            int m = ReadNonNegative("Enter minutes (0..59):", 59);
            int s = ReadNonNegative("Enter seconds (0..59):", 59);

            try
            {
                int total = Functions.TotalSeconds(h, m, s);

                Console.WriteLine("\nSummary:");
                Console.WriteLine($"  Input time     : {Functions.FormatHms(h, m, s)}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  Total seconds  : {total:N0}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: {ex.Message}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nPress any key to exit...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}