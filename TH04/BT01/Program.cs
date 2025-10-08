using System.Globalization;

namespace BT01
{
    internal class Program
    {
        static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Float,
                                    CultureInfo.CurrentCulture, out double v) && v > 0)
                    return v;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Invalid input. Please enter a positive number.\n");
                Console.ResetColor();
            }
        }

        static void Main()
        {
            Console.Title = "BT01 - Triangle Perimeter & Area";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===== BT01: TRIANGLE PERIMETER & AREA =====\n");
            Console.ResetColor();

            double a = ReadDouble("Enter side a (>0): ");
            double b = ReadDouble("Enter side b (>0): ");
            double c = ReadDouble("Enter side c (>0): ");

            try
            {
                if (!Functions.IsValidTriangle(a, b, c))
                    throw new ArgumentException("The three sides do not satisfy the triangle inequality.");

                double p = Functions.Perimeter(a, b, c);
                double s = Functions.Area(a, b, c);
                double semi = p / 2.0;

                Console.WriteLine("\nSummary:");
                Console.WriteLine($"  Sides          : a={a}, b={b}, c={c}");
                Console.WriteLine($"  Perimeter (P)  : {p:N3}");
                Console.WriteLine($"  Semi-perimeter : {semi:N3}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  Area (S)       : {s:N3}");
                Console.ResetColor();

                Console.WriteLine("\nFormulas:");
                Console.WriteLine("  P = a + b + c");
                Console.WriteLine("  S = √(p(p-a)(p-b)(p-c)), where p = P / 2");
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
