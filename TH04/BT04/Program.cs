namespace BT04
{
    internal class Program
    {
        static int ReadTwoDigitNumber()
        {
            while (true)
            {
                Console.Write("Enter a two-digit number (10..99): ");
                if (int.TryParse(Console.ReadLine(), out int v) && Math.Abs(v) >= 10 && Math.Abs(v) <= 99)
                    return v;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid input. Please enter a two-digit integer (10..99).\n");
                Console.ResetColor();
            }
        }

        static void Main()
        {
            Console.Title = "BT04 - Sum of Two Digits";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===== BT04: SUM OF TWO DIGITS =====\n");
            Console.ResetColor();

            int num = ReadTwoDigitNumber();

            try
            {
                int sum2 = Functions.SumTwoDigits(num);
                int sumAll = Functions.SumAllDigits(num);

                Console.WriteLine("\nSummary:");
                Console.WriteLine($"  Input number        : {num}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  Sum of two digits   : {sum2}");
                Console.ResetColor();
                Console.WriteLine($"  (All-digit sum)     : {sumAll}  // Optional check");
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
