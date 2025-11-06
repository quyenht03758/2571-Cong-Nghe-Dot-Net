/*
 * BT01 - Display Minimum Values of Data Types
 * This program displays the MinValue property of numeric data types in C#
 */

namespace BT01;

class Program
{
    static void Main(string[] args)
    {
        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        MINIMUM VALUES OF NUMERIC DATA TYPES            ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Display minimum value of decimal (high-precision decimal number)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("decimal:  ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{decimal.MinValue,50}");

        // Display minimum value of double (double-precision floating-point)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("double:   ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{double.MinValue,50}");

        // Display minimum value of float (single-precision floating-point)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("float:    ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{float.MinValue,50}");

        // Display minimum value of int (32-bit signed integer)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("int:      ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{int.MinValue,50}");

        // Display minimum value of long (64-bit signed integer)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("long:     ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{long.MinValue,50}");

        // Display minimum value of ulong (64-bit unsigned integer)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("ulong:    ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{ulong.MinValue,50}");

        // Display minimum value of short (16-bit signed integer)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("short:    ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{short.MinValue,50}");

        // Display minimum value of ushort (16-bit unsigned integer)
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("ushort:   ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{ushort.MinValue,50}");

        // End program
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
