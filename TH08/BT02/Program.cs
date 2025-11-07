/*
 * BT02 - Stream Writing to TXT and XML Files
 * This program demonstrates writing car array data to both .txt and .xml files using streams
 */

using System.Xml.Serialization;

namespace BT02;

class Program
{
    static void Main(string[] args)
    {
        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║          CAR DATA WRITER (TXT & XML)                   ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        // Car array data
        string[] cars = { "Honda", "BMW", "Ford", "Mazda" };

        // Display car data
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Car Array Data:");
        Console.ResetColor();
        for (int i = 0; i < cars.Length; i++)
        {
            Console.WriteLine($"  [{i}] {cars[i]}");
        }
        Console.WriteLine();

        try
        {
            // ========== Write to TXT file using StreamWriter ==========
            string txtFilename = "cars.txt";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Writing to TXT file...");
            Console.ResetColor();

            using (StreamWriter writer = new StreamWriter(txtFilename))
            {
                writer.WriteLine("Car List");
                writer.WriteLine("========");
                for (int i = 0; i < cars.Length; i++)
                {
                    writer.WriteLine($"{i + 1}. {cars[i]}");
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Successfully written to {txtFilename}");
            Console.ResetColor();
            Console.WriteLine($"  Path: {Path.GetFullPath(txtFilename)}");
            Console.WriteLine();

            // Display TXT file content
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("TXT File Content:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(File.ReadAllText(txtFilename));
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.WriteLine();

            // ========== Write to XML file using XmlSerializer ==========
            string xmlFilename = "cars.xml";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Writing to XML file...");
            Console.ResetColor();

            // Create a CarList object for XML serialization
            CarList carList = new CarList { Cars = cars.ToList() };

            XmlSerializer serializer = new XmlSerializer(typeof(CarList));
            using (StreamWriter writer = new StreamWriter(xmlFilename))
            {
                serializer.Serialize(writer, carList);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Successfully written to {xmlFilename}");
            Console.ResetColor();
            Console.WriteLine($"  Path: {Path.GetFullPath(xmlFilename)}");
            Console.WriteLine();

            // Display XML file content
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("XML File Content:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(File.ReadAllText(xmlFilename));
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n⚠ Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Press any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }
}

/// <summary>
/// Helper class for XML serialization of car list
/// </summary>
[XmlRoot("CarList")]
public class CarList
{
    [XmlElement("Car")]
    public List<string> Cars { get; set; } = new List<string>();
}
