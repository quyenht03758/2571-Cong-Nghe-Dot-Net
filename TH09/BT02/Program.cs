/*
 * BT02 - Filter and Sort Products
 * This program filters products where UnitsInStock >= UnitsOnOrder
 * and sorts them in descending order by UnitPrice
 */

using System.Text;
using Microsoft.EntityFrameworkCore;
using NorthwindLib;

namespace BT02;

class Program
{
    static void Main(string[] args)
    {
        // Set console properties
        Console.Title = "BT02 - Filter and Sort Products by Stock and Price";
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        FILTER AND SORT PRODUCTS BY STOCK AND PRICE                ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            using var context = new NorthwindContext();

            // Query products where UnitsInStock >= UnitsOnOrder
            // Sort by UnitPrice descending
            // Include supplier information
            var filteredProducts = context.Products
                .Include(p => p.Supplier)
                .Where(p => p.UnitsInStock >= p.UnitsOnOrder)
                .OrderByDescending(p => p.UnitPrice)
                .ToList();

            // Display results
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Filter Criteria:");
            Console.WriteLine("  • UnitsInStock >= UnitsOnOrder");
            Console.WriteLine("  • Sorted by UnitPrice (Descending)");
            Console.WriteLine();
            Console.WriteLine($"Total Products Found: {filteredProducts.Count}");
            Console.ResetColor();
            Console.WriteLine();

            // Display table header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┌────┬─────────────────────────────────────────┬─────────────────────────┬───────────┬───────────┬─────────┐");
            Console.WriteLine("│ No │ Product Name                            │ Supplier                │   Price   │   Stock   │  Ordered│");
            Console.WriteLine("├────┼─────────────────────────────────────────┼─────────────────────────┼───────────┼───────────┼─────────┤");
            Console.ResetColor();

            // Display products
            int no = 1;
            foreach (var product in filteredProducts)
            {
                string productName = product.ProductName.Length > 39
                    ? product.ProductName.Substring(0, 36) + "..."
                    : product.ProductName;

                string supplierName = product.Supplier?.CompanyName ?? "N/A";
                if (supplierName.Length > 23)
                    supplierName = supplierName.Substring(0, 20) + "...";

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"│{no,3} │ {productName,-40}│ {supplierName,-24}│ {product.UnitPrice,9:C2}│ {product.UnitsInStock,9} │{product.UnitsOnOrder,8} │");
                Console.ResetColor();

                no++;
            }

            // Display table footer
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("└────┴─────────────────────────────────────────┴─────────────────────────┴───────────┴───────────┴─────────┘");
            Console.ResetColor();

            // Summary statistics
            var avgPrice = filteredProducts.Average(p => p.UnitPrice ?? 0);
            var maxPrice = filteredProducts.Max(p => p.UnitPrice ?? 0);
            var minPrice = filteredProducts.Min(p => p.UnitPrice ?? 0);
            var totalStockValue = filteredProducts.Sum(p => (p.UnitPrice ?? 0) * (p.UnitsInStock ?? 0));

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("STATISTICS");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine($"Highest Price:           {maxPrice:C2}");
            Console.WriteLine($"Lowest Price:            {minPrice:C2}");
            Console.WriteLine($"Average Price:           {avgPrice:C2}");
            Console.WriteLine($"Total Stock Value:       {totalStockValue:C2}");

            // Products with highest and lowest prices
            var highestPriceProduct = filteredProducts.First();
            var lowestPriceProduct = filteredProducts.Last();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Most Expensive:  {highestPriceProduct.ProductName} ({highestPriceProduct.UnitPrice:C2})");
            Console.WriteLine($"Least Expensive: {lowestPriceProduct.ProductName} ({lowestPriceProduct.UnitPrice:C2})");
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

        // End program
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
