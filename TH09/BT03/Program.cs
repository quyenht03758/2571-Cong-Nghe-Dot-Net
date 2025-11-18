/*
 * BT03 - CRUD Operations for Suppliers
 * This program performs Create, Read, Update, and Delete operations on Suppliers
 */

using System.Text;
using Microsoft.EntityFrameworkCore;
using NorthwindLib;

namespace BT03;

class Program
{
    static void Main(string[] args)
    {
        // Set console properties
        Console.Title = "BT03 - Supplier Management System (CRUD)";
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        bool exit = false;
        while (!exit)
        {
            Console.Clear();

            // Display program header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        SUPPLIER MANAGEMENT SYSTEM (CRUD)                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            // Display menu
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("MENU:");
            Console.WriteLine("────────────────────────────────────────────────────────────────────");
            Console.ResetColor();
            Console.WriteLine("1. View All Suppliers");
            Console.WriteLine("2. Add New Supplier");
            Console.WriteLine("3. Update Supplier");
            Console.WriteLine("4. Delete Supplier");
            Console.WriteLine("5. Search Supplier by Name");
            Console.WriteLine("6. Exit");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Select an option (1-6): ");
            Console.ResetColor();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllSuppliers();
                    break;
                case "2":
                    AddNewSupplier();
                    break;
                case "3":
                    UpdateSupplier();
                    break;
                case "4":
                    DeleteSupplier();
                    break;
                case "5":
                    SearchSupplier();
                    break;
                case "6":
                    exit = true;
                    break;
                default:
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice! Please enter 1-6.");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Press any key to continue...");
                    Console.ResetColor();
                    Console.ReadKey();
                    break;
            }
        }

        // Exit message
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Thank you for using Supplier Management System!");
        Console.ResetColor();
    }

    /// <summary>
    /// View all suppliers
    /// </summary>
    static void ViewAllSuppliers()
    {
        try
        {
            using var context = new NorthwindContext();
            var suppliers = context.Suppliers
                .Include(s => s.Products)
                .OrderBy(s => s.CompanyName)
                .ToList();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Total Suppliers: {suppliers.Count}");
            Console.ResetColor();
            Console.WriteLine();

            // Display table header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┌─────┬──────────────────────────────────┬─────────────────────┬───────────────────┬──────────────┐");
            Console.WriteLine("│  ID │ Company Name                     │ Contact             │ Country           │ Products     │");
            Console.WriteLine("├─────┼──────────────────────────────────┼─────────────────────┼───────────────────┼──────────────┤");
            Console.ResetColor();

            foreach (var supplier in suppliers)
            {
                string companyName = supplier.CompanyName.Length > 32
                    ? supplier.CompanyName.Substring(0, 29) + "..."
                    : supplier.CompanyName;

                string contactName = (supplier.ContactName ?? "N/A").Length > 19
                    ? (supplier.ContactName ?? "N/A").Substring(0, 16) + "..."
                    : (supplier.ContactName ?? "N/A");

                string country = (supplier.Country ?? "N/A").Length > 17
                    ? (supplier.Country ?? "N/A").Substring(0, 14) + "..."
                    : (supplier.Country ?? "N/A");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"│{supplier.SupplierID,4} │ {companyName,-33}│ {contactName,-20}│ {country,-18}│ {supplier.Products.Count,12} │");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("└─────┴──────────────────────────────────┴─────────────────────┴───────────────────┴──────────────┘");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }

    /// <summary>
    /// Add a new supplier
    /// </summary>
    static void AddNewSupplier()
    {
        try
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ADD NEW SUPPLIER");
            Console.WriteLine("────────────────────────────────────────────────────────────────────");
            Console.ResetColor();

            var supplier = new Supplier();

            Console.Write("Company Name: ");
            supplier.CompanyName = Console.ReadLine() ?? "";

            Console.Write("Contact Name: ");
            supplier.ContactName = Console.ReadLine();

            Console.Write("Contact Title: ");
            supplier.ContactTitle = Console.ReadLine();

            Console.Write("Address: ");
            supplier.Address = Console.ReadLine();

            Console.Write("City: ");
            supplier.City = Console.ReadLine();

            Console.Write("Region: ");
            supplier.Region = Console.ReadLine();

            Console.Write("Postal Code: ");
            supplier.PostalCode = Console.ReadLine();

            Console.Write("Country: ");
            supplier.Country = Console.ReadLine();

            Console.Write("Phone: ");
            supplier.Phone = Console.ReadLine();

            Console.Write("Fax: ");
            supplier.Fax = Console.ReadLine();

            Console.Write("Home Page: ");
            supplier.HomePage = Console.ReadLine();

            // Validate required field
            if (string.IsNullOrWhiteSpace(supplier.CompanyName))
            {
                throw new ArgumentException("Company Name is required!");
            }

            // Add to database
            using var context = new NorthwindContext();
            context.Suppliers.Add(supplier);
            context.SaveChanges();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Supplier added successfully with ID: {supplier.SupplierID}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }

    /// <summary>
    /// Update an existing supplier
    /// </summary>
    static void UpdateSupplier()
    {
        try
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("UPDATE SUPPLIER");
            Console.WriteLine("────────────────────────────────────────────────────────────────────");
            Console.ResetColor();

            Console.Write("Enter Supplier ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int supplierId))
            {
                throw new ArgumentException("Invalid Supplier ID!");
            }

            using var context = new NorthwindContext();
            var supplier = context.Suppliers.Find(supplierId);

            if (supplier == null)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Supplier with ID {supplierId} not found!");
                Console.ResetColor();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // Display current information
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Current Information:");
            Console.ResetColor();
            Console.WriteLine($"Company Name: {supplier.CompanyName}");
            Console.WriteLine($"Contact: {supplier.ContactName ?? "N/A"}");
            Console.WriteLine($"Country: {supplier.Country ?? "N/A"}");
            Console.WriteLine($"Phone: {supplier.Phone ?? "N/A"}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Enter new values (press Enter to keep current value):");
            Console.ResetColor();

            Console.Write($"Company Name [{supplier.CompanyName}]: ");
            string? newCompanyName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newCompanyName))
                supplier.CompanyName = newCompanyName;

            Console.Write($"Contact Name [{supplier.ContactName ?? ""}]: ");
            string? newContactName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newContactName))
                supplier.ContactName = newContactName;

            Console.Write($"Contact Title [{supplier.ContactTitle ?? ""}]: ");
            string? newContactTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newContactTitle))
                supplier.ContactTitle = newContactTitle;

            Console.Write($"Address [{supplier.Address ?? ""}]: ");
            string? newAddress = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAddress))
                supplier.Address = newAddress;

            Console.Write($"City [{supplier.City ?? ""}]: ");
            string? newCity = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newCity))
                supplier.City = newCity;

            Console.Write($"Country [{supplier.Country ?? ""}]: ");
            string? newCountry = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newCountry))
                supplier.Country = newCountry;

            Console.Write($"Phone [{supplier.Phone ?? ""}]: ");
            string? newPhone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPhone))
                supplier.Phone = newPhone;

            // Save changes
            context.SaveChanges();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Supplier updated successfully!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }

    /// <summary>
    /// Delete a supplier
    /// </summary>
    static void DeleteSupplier()
    {
        try
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("DELETE SUPPLIER");
            Console.WriteLine("────────────────────────────────────────────────────────────────────");
            Console.ResetColor();

            Console.Write("Enter Supplier ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int supplierId))
            {
                throw new ArgumentException("Invalid Supplier ID!");
            }

            using var context = new NorthwindContext();
            var supplier = context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefault(s => s.SupplierID == supplierId);

            if (supplier == null)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Supplier with ID {supplierId} not found!");
                Console.ResetColor();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // Display supplier information
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Supplier to delete:");
            Console.ResetColor();
            Console.WriteLine($"ID: {supplier.SupplierID}");
            Console.WriteLine($"Company Name: {supplier.CompanyName}");
            Console.WriteLine($"Contact: {supplier.ContactName ?? "N/A"}");
            Console.WriteLine($"Country: {supplier.Country ?? "N/A"}");
            Console.WriteLine($"Products: {supplier.Products.Count}");
            Console.WriteLine();

            // Confirm deletion
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Are you sure you want to delete this supplier? (Y/N): ");
            Console.ResetColor();
            string? confirm = Console.ReadLine();

            if (confirm?.ToUpper() == "Y")
            {
                context.Suppliers.Remove(supplier);
                context.SaveChanges();

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Supplier deleted successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Deletion cancelled.");
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

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }

    /// <summary>
    /// Search for suppliers by company name
    /// </summary>
    static void SearchSupplier()
    {
        try
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("SEARCH SUPPLIER");
            Console.WriteLine("────────────────────────────────────────────────────────────────────");
            Console.ResetColor();

            Console.Write("Enter search keyword: ");
            string? keyword = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Search keyword cannot be empty!");
            }

            using var context = new NorthwindContext();
            var suppliers = context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.CompanyName.Contains(keyword))
                .OrderBy(s => s.CompanyName)
                .ToList();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Found {suppliers.Count} supplier(s)");
            Console.ResetColor();
            Console.WriteLine();

            if (suppliers.Any())
            {
                foreach (var supplier in suppliers)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"┌─────────────────────────────────────────────────────────");
                    Console.WriteLine($"│ ID: {supplier.SupplierID}");
                    Console.WriteLine($"│ Company: {supplier.CompanyName}");
                    Console.WriteLine($"│ Contact: {supplier.ContactName ?? "N/A"}");
                    Console.WriteLine($"│ Country: {supplier.Country ?? "N/A"}");
                    Console.WriteLine($"│ Phone: {supplier.Phone ?? "N/A"}");
                    Console.WriteLine($"│ Products: {supplier.Products.Count}");
                    Console.WriteLine($"└─────────────────────────────────────────────────────────");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
