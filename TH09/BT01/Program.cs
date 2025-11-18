/*
 * BT01 - Query Suppliers and Their Products
 * This program queries suppliers and displays what products they supply
 */

using System.Text;
using Microsoft.EntityFrameworkCore;
using NorthwindLib;

namespace BT01;

class Program
{
    static void Main(string[] args)
    {
        // Set console properties
        Console.Title = "BT01 - Suppliers and Their Products Query";
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        SUPPLIERS AND THEIR PRODUCTS QUERY                          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            using var context = new NorthwindContext();

            // Query suppliers with their products using Include for eager loading
            var suppliersWithProducts = context.Suppliers
                .Include(s => s.Products)
                .OrderBy(s => s.CompanyName)
                .ToList();

            // Display results
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Total Suppliers: {suppliersWithProducts.Count}");
            Console.ResetColor();
            Console.WriteLine();

            foreach (var supplier in suppliersWithProducts)
            {
                // Display supplier information
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"┌─────────────────────────────────────────────────────────────────");
                Console.WriteLine($"│ Supplier ID: {supplier.SupplierID}");
                Console.WriteLine($"│ Company: {supplier.CompanyName}");
                Console.WriteLine($"│ Contact: {supplier.ContactName ?? "N/A"}");
                Console.WriteLine($"│ Country: {supplier.Country ?? "N/A"}");
                Console.WriteLine($"│ Phone: {supplier.Phone ?? "N/A"}");
                Console.ResetColor();

                // Display products
                if (supplier.Products.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"│ Products ({supplier.Products.Count}):");
                    Console.ResetColor();

                    foreach (var product in supplier.Products.OrderBy(p => p.ProductName))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"│   • {product.ProductName,-35} " +
                            $"Price: {product.UnitPrice:C2,-10} " +
                            $"Stock: {product.UnitsInStock,5}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"│   (No products)");
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"└─────────────────────────────────────────────────────────────────");
                Console.ResetColor();
                Console.WriteLine();
            }

            // Summary statistics
            var totalProducts = suppliersWithProducts.Sum(s => s.Products.Count);
            var suppliersWithNoProducts = suppliersWithProducts.Count(s => !s.Products.Any());

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════════════════════");
            Console.WriteLine("SUMMARY");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine($"Total Suppliers: {suppliersWithProducts.Count}");
            Console.WriteLine($"Total Products: {totalProducts}");
            Console.WriteLine($"Suppliers with no products: {suppliersWithNoProducts}");
            Console.WriteLine($"Average products per supplier: {(double)totalProducts / suppliersWithProducts.Count:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Lỗi: {ex.Message}");
            Console.ResetColor();

            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
            }
        }

        // End program
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
