/*
 * TH10 - Northwind Customer Query Program
 * This program performs two tasks:
 * 1. List customers by city (user input)
 * 2. List all distinct cities from customers
 */

using System.Text;
using NorthwindLib;

namespace BT01;

class Program
{
    static void Main(string[] args)
    {
        // Set console properties
        Console.Title = "TH10 - Northwind Customer Query Program";
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            // Display program header
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        NORTHWIND CUSTOMER QUERY PROGRAM                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            // Display menu
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MENU:");
            Console.ResetColor();
            Console.WriteLine("1. List customers by city");
            Console.WriteLine("2. List all distinct cities");
            Console.WriteLine("3. Exit");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Select an option (1-3): ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ListCustomersByCity();
                    break;
                case "2":
                    ListAllDistinctCities();
                    break;
                case "3":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Thank you for using the program!");
                    Console.ResetColor();
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option! Please select 1, 2, or 3.");
                    Console.ResetColor();
                    break;
            }

            // Wait for user to continue
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Task 1: List customers by city (user input)
    /// </summary>
    static void ListCustomersByCity()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine("TASK 1: LIST CUSTOMERS BY CITY");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            using var context = new NorthwindContext();

            // Prompt for city name
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter the name of a city: ");
            Console.ResetColor();
            string? cityInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cityInput))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("City name cannot be empty!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine();

            // Query customers by city (case-insensitive)
            var customers = context.Customers
                .Where(c => c.City != null && c.City.ToLower() == cityInput.ToLower())
                .OrderBy(c => c.CompanyName)
                .ToList();

            // Display results
            if (customers.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No customers found in {cityInput}.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"There are {customers.Count} customers in {cityInput}:");
                Console.ResetColor();

                foreach (var customer in customers)
                {
                    Console.WriteLine(customer.CompanyName);
                }

                // Display detailed table
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Detailed Information:");
                Console.ResetColor();
                Console.WriteLine();

                // Display table header
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("┌────┬─────────────────────────────────────────┬─────────────────────────┬─────────────────┐");
                Console.WriteLine("│ No │ Company Name                            │ Contact Name            │ Phone           │");
                Console.WriteLine("├────┼─────────────────────────────────────────┼─────────────────────────┼─────────────────┤");
                Console.ResetColor();

                // Display customers
                int no = 1;
                foreach (var customer in customers)
                {
                    string companyName = customer.CompanyName.Length > 39
                        ? customer.CompanyName.Substring(0, 36) + "..."
                        : customer.CompanyName;

                    string contactName = customer.ContactName ?? "N/A";
                    if (contactName.Length > 23)
                        contactName = contactName.Substring(0, 20) + "...";

                    string phone = customer.Phone ?? "N/A";
                    if (phone.Length > 15)
                        phone = phone.Substring(0, 12) + "...";

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"│{no,3} │ {companyName,-40}│ {contactName,-24}│ {phone,-16}│");
                    Console.ResetColor();

                    no++;
                }

                // Display table footer
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("└────┴─────────────────────────────────────────┴─────────────────────────┴─────────────────┘");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }

    /// <summary>
    /// Task 2: List all distinct cities from customers
    /// </summary>
    static void ListAllDistinctCities()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
        Console.WriteLine("TASK 2: LIST ALL DISTINCT CITIES");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            using var context = new NorthwindContext();

            // Query all distinct cities (non-null) and order alphabetically
            var cities = context.Customers
                .Where(c => c.City != null)
                .Select(c => c.City!)
                .Distinct()
                .OrderBy(city => city)
                .ToList();

            // Display results
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Total Unique Cities: {cities.Count}");
            Console.ResetColor();
            Console.WriteLine();

            // Display cities in a formatted manner (comma-separated)
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cities:");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;

            // Display all cities on one line, comma-separated (as per requirement)
            Console.WriteLine(string.Join(", ", cities));

            Console.ResetColor();
            Console.WriteLine();

            // Display statistics (bonus feature)
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("STATISTICS");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.ResetColor();

            // Count customers per city
            var citiesWithCount = context.Customers
                .Where(c => c.City != null)
                .GroupBy(c => c.City)
                .Select(g => new { City = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Top 10 Cities by Customer Count:");
            Console.ResetColor();
            Console.WriteLine();

            // Display table header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┌────┬─────────────────────────────────────────┬──────────────┐");
            Console.WriteLine("│ No │ City                                    │   Customers  │");
            Console.WriteLine("├────┼─────────────────────────────────────────┼──────────────┤");
            Console.ResetColor();

            // Display cities with counts
            int no = 1;
            foreach (var item in citiesWithCount)
            {
                string cityName = (item.City ?? "Unknown").Length > 39
                    ? (item.City ?? "Unknown").Substring(0, 36) + "..."
                    : (item.City ?? "Unknown");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"│{no,3} │ {cityName,-40}│{item.Count,13} │");
                Console.ResetColor();

                no++;
            }

            // Display table footer
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("└────┴─────────────────────────────────────────┴──────────────┘");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }
    }
}
