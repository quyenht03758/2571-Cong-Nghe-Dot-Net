namespace BT02
{
    internal class Program
    {
        static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max)
                    return v;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Invalid input. Please enter an integer in [{min}..{max}].\n");
                Console.ResetColor();
            }
        }

        static void Main()
        {
            Console.Title = "BT02 - Multiplication Table (Single Number)";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===== BT02: MULTIPLICATION TABLE (1 NUMBER) =====\n");
            Console.ResetColor();

            int n = ReadIntInRange("Enter a number (1..9): ", 1, 9);

            try
            {
                Functions.MultiplicationTable(n);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nDone.");
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
