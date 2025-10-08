using System;

namespace BT05
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "BT05 - Electricity Bill Calculation";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===== BT05: ELECTRICITY BILL CALCULATION =====");
            Console.ResetColor();

            try
            {
                // Ask the user for input
                Console.Write("Enter electricity usage (kWh): ");
                int kWh = int.Parse(Console.ReadLine() ?? "0");

                if (kWh < 0)
                    throw new ArgumentOutOfRangeException(nameof(kWh), "Electricity usage must be non-negative.");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nElectricity usage: {kWh} kWh");
                Console.ResetColor();

                // Calculate and show breakdown
                double total = Functions.ElectricityBill(kWh);

                // Display total
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nTOTAL AMOUNT TO PAY: {total:N0} VND");
                Console.ResetColor();

                // Reference price table
                Console.WriteLine("\nRate per tier (VND/kWh):");
                Console.WriteLine(" Tier 1: 0–50    → 1,678");
                Console.WriteLine(" Tier 2: 51–100  → 1,734");
                Console.WriteLine(" Tier 3: 101–200 → 2,014");
                Console.WriteLine(" Tier 4: 201–300 → 2,536");
                Console.WriteLine(" Tier 5: 301–400 → 2,834");
                Console.WriteLine(" Tier 6: >400    → 2,927");
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
