using System;

namespace BT05
{
    internal static class Functions
    {
        /// <summary>
        /// Calculates the electricity bill for a given kWh consumption.
        /// Displays the amount for each level (tier) before showing the total.
        /// </summary>
        /// <param name="kWh">Electricity usage in kilowatt-hours</param>
        /// <returns>Total bill amount in VND</returns>
        public static double ElectricityBill(int kWh)
        {
            double total = 0;
            double cost;

            Console.WriteLine("\nElectricity charge by tier:");
            Console.WriteLine(new string('-', 55));

            // Tier 1: 0–50 kWh
            if (kWh <= 50)
            {
                cost = kWh * 1678;
                Console.WriteLine($"Tier 1 (0–50):   {kWh,3} kWh × 1,678 = {cost,12:N0} VND");
                total += cost;
                Console.WriteLine(new string('-', 55));
                return total;
            }
            else
            {
                cost = 50 * 1678;
                Console.WriteLine($"Tier 1 (0–50):   50 kWh × 1,678 = {cost,12:N0} VND");
                total += cost;
            }

            // Tier 2: 51–100 kWh
            if (kWh > 50)
            {
                int used = Math.Min(kWh - 50, 50);
                cost = used * 1734;
                Console.WriteLine($"Tier 2 (51–100): {used,3} kWh × 1,734 = {cost,12:N0} VND");
                total += cost;
            }

            // Tier 3: 101–200 kWh
            if (kWh > 100)
            {
                int used = Math.Min(kWh - 100, 100);
                cost = used * 2014;
                Console.WriteLine($"Tier 3 (101–200):{used,3} kWh × 2,014 = {cost,12:N0} VND");
                total += cost;
            }

            // Tier 4: 201–300 kWh
            if (kWh > 200)
            {
                int used = Math.Min(kWh - 200, 100);
                cost = used * 2536;
                Console.WriteLine($"Tier 4 (201–300):{used,3} kWh × 2,536 = {cost,12:N0} VND");
                total += cost;
            }

            // Tier 5: 301–400 kWh
            if (kWh > 300)
            {
                int used = Math.Min(kWh - 300, 100);
                cost = used * 2834;
                Console.WriteLine($"Tier 5 (301–400):{used,3} kWh × 2,834 = {cost,12:N0} VND");
                total += cost;
            }

            // Tier 6: >400 kWh
            if (kWh > 400)
            {
                int used = kWh - 400;
                cost = used * 2927;
                Console.WriteLine($"Tier 6 (>400):   {used,3} kWh × 2,927 = {cost,12:N0} VND");
                total += cost;
            }

            Console.WriteLine(new string('-', 55));
            return total;
        }
    }
}
