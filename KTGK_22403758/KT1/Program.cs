/*
 * KT1 - 2D Array Difference Calculator
 * This program calculates the difference between two 2D arrays
 */

using System.Text;

namespace KT1;

class Program
{
    static void Main(string[] args)
    {
        // Set console encoding to UTF-8 to support special characters
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         2D ARRAY DIFFERENCE CALCULATOR                 ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            // Get array dimensions from user
            var (rows, cols) = InputArrayDimensions();

            // Input first array
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                    INPUT ARRAY 1                      ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ResetColor();
            int[,] array1 = InputArray(rows, cols, "Array 1");

            // Input second array
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("                    INPUT ARRAY 2                      ");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ResetColor();
            int[,] array2 = InputArray(rows, cols, "Array 2");

            // Display first array
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Array 1:");
            Console.ResetColor();
            DisplayArray(array1);

            Console.WriteLine();

            // Display second array
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Array 2:");
            Console.ResetColor();
            DisplayArray(array2);

            Console.WriteLine();

            // Calculate difference of two arrays
            int[,] differenceArray = CalculateArrayDifference(array1, array2);

            // Display result
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Result Array (Array1 - Array2):");
            Console.ResetColor();
            DisplayArray(differenceArray);
        }
        catch (FormatException)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Invalid number format!");
            Console.ResetColor();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        // End program
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("════════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Calculates the difference between two 2D arrays
    /// </summary>
    /// <param name="array1">First array</param>
    /// <param name="array2">Second array</param>
    /// <returns>Resulting array containing the difference</returns>
    static int[,] CalculateArrayDifference(int[,] array1, int[,] array2)
    {
        int rows = array1.GetLength(0);
        int cols = array1.GetLength(1);

        int[,] resultArray = new int[rows, cols];

        // Subtract each element of array2 from corresponding element in array1
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                resultArray[i, j] = array1[i, j] - array2[i, j];
            }
        }

        return resultArray;
    }

    /// <summary>
    /// Displays a 2D array with formatted output
    /// </summary>
    /// <param name="array">The array to display</param>
    static void DisplayArray(int[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < rows; i++)
        {
            Console.Write("   ");
            for (int j = 0; j < cols; j++)
            {
                Console.Write($"{array[i, j],6} ");
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Prompts user to input dimensions for the 2D arrays
    /// </summary>
    /// <returns>Tuple containing number of rows and columns</returns>
    static (int rows, int cols) InputArrayDimensions()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Enter array dimensions:");
        Console.ResetColor();
        Console.WriteLine("────────────────────────────────────────────────────────");

        // Get number of rows
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Number of rows: ");
        Console.ResetColor();
        string? rowInput = Console.ReadLine();
        int rows = int.Parse(rowInput ?? "0");

        if (rows <= 0)
        {
            throw new ArgumentException("Number of rows must be greater than 0!");
        }

        // Get number of columns
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Number of columns: ");
        Console.ResetColor();
        string? colInput = Console.ReadLine();
        int cols = int.Parse(colInput ?? "0");

        if (cols <= 0)
        {
            throw new ArgumentException("Number of columns must be greater than 0!");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Array dimensions set to {rows}x{cols}");
        Console.ResetColor();

        return (rows, cols);
    }

    /// <summary>
    /// Prompts user to input elements for a 2D array
    /// </summary>
    /// <param name="rows">Number of rows</param>
    /// <param name="cols">Number of columns</param>
    /// <param name="arrayName">Name of the array for display purposes</param>
    /// <returns>The populated 2D array</returns>
    static int[,] InputArray(int rows, int cols, string arrayName)
    {
        int[,] array = new int[rows, cols];

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Enter elements for {arrayName} ({rows}x{cols}):");
        Console.ResetColor();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"Element [{i},{j}]: ");
                Console.ResetColor();
                string? input = Console.ReadLine();
                array[i, j] = int.Parse(input ?? "0");
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{arrayName} input completed!");
        Console.ResetColor();

        return array;
    }
}
