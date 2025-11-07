/*
 * BT03 - XML Serialization and Deserialization
 * This program demonstrates XML serialization with Employee (NhanVien) and Dependent (ThanNhan) data
 */

using System.Xml.Serialization;

namespace BT03;

class Program
{
    static void Main(string[] args)
    {
        // Display program header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║      XML SERIALIZATION & DESERIALIZATION DEMO          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        string xmlFilename = "company_data.xml";

        // ========== PART 1: SERIALIZATION ==========
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("   PART 1: XML SERIALIZATION");
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            // Create Company data structure
            Company company = CreateCompanyData();

            // Display data before serialization
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Employee Data:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            foreach (var emp in company.Employees)
            {
                Console.WriteLine($"SSN: {emp.SSN}, Name: {emp.FName} {emp.LName}, " +
                                $"BDate: {emp.BDate:MM/dd/yyyy}, Salary: ${emp.Salary:N0}");
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Dependent Data:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            foreach (var dep in company.Dependents)
            {
                Console.WriteLine($"ESSN: {dep.ESSN}, Dependent Name: {dep.DependentName}");
            }
            Console.WriteLine();

            // Serialize to XML
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Serializing data to XML...");
            Console.ResetColor();

            XmlSerializer serializer = new XmlSerializer(typeof(Company));
            using (StreamWriter writer = new StreamWriter(xmlFilename))
            {
                serializer.Serialize(writer, company);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ Successfully serialized to {xmlFilename}");
            Console.ResetColor();
            Console.WriteLine($"  Path: {Path.GetFullPath(xmlFilename)}");
            Console.WriteLine();

            // Display XML content
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Generated XML Content:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.White;
            string xmlContent = File.ReadAllText(xmlFilename);
            Console.WriteLine(xmlContent);
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.WriteLine();

            // ========== PART 2: DESERIALIZATION ==========
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.WriteLine("   PART 2: XML DESERIALIZATION");
            Console.WriteLine("═══════════════════════════════════════════════════════");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Deserializing data from {xmlFilename}...");
            Console.ResetColor();

            // Deserialize from XML
            Company deserializedCompany;
            using (StreamReader reader = new StreamReader(xmlFilename))
            {
                deserializedCompany = (Company)serializer.Deserialize(reader)!;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Successfully deserialized from XML");
            Console.ResetColor();
            Console.WriteLine();

            // Display deserialized data
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Deserialized Employee Data:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.WriteLine($"{"SSN",-12} {"First Name",-12} {"Last Name",-12} {"Birth Date",-12} {"Salary",10}");
            Console.WriteLine("────────────────────────────────────────────────────────");
            foreach (var emp in deserializedCompany.Employees)
            {
                Console.WriteLine($"{emp.SSN,-12} {emp.FName,-12} {emp.LName,-12} {emp.BDate:MM/dd/yyyy}  ${emp.Salary,9:N0}");
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Deserialized Dependent Data:");
            Console.ResetColor();
            Console.WriteLine("────────────────────────────────────────────────────────");
            Console.WriteLine($"{"ESSN",-12} {"Dependent Name",-20}");
            Console.WriteLine("────────────────────────────────────────────────────────");
            foreach (var dep in deserializedCompany.Dependents)
            {
                Console.WriteLine($"{dep.ESSN,-12} {dep.DependentName,-20}");
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Serialization and Deserialization completed successfully!");
            Console.ResetColor();
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

    /// <summary>
    /// Creates sample company data with employees and dependents
    /// </summary>
    static Company CreateCompanyData()
    {
        Company company = new Company();

        // Add Employees (NhanVien)
        company.Employees.Add(new NhanVien
        {
            SSN = "123456789",
            FName = "John",
            LName = "Smith",
            BDate = new DateTime(1955, 1, 9),
            Salary = 30000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "333445555",
            FName = "Franklin",
            LName = "Wong",
            BDate = new DateTime(1945, 12, 8),
            Salary = 40000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "453453453",
            FName = "Joyce",
            LName = "English",
            BDate = new DateTime(1962, 7, 31),
            Salary = 25000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "666884444",
            FName = "Ramesh",
            LName = "Narayan",
            BDate = new DateTime(1952, 9, 15),
            Salary = 38000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "888665555",
            FName = "James",
            LName = "Borg",
            BDate = new DateTime(1927, 11, 10),
            Salary = 55000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "987654321",
            FName = "Jennifer",
            LName = "Wallace",
            BDate = new DateTime(1931, 6, 20),
            Salary = 43000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "987987987",
            FName = "Ahmad",
            LName = "Jabbar",
            BDate = new DateTime(1959, 3, 29),
            Salary = 25000
        });
        company.Employees.Add(new NhanVien
        {
            SSN = "999887777",
            FName = "Alicia",
            LName = "Zelaya",
            BDate = new DateTime(1958, 7, 19),
            Salary = 25000
        });

        // Add Dependents (ThanNhan)
        company.Dependents.Add(new ThanNhan { ESSN = "123456789", DependentName = "Alice" });
        company.Dependents.Add(new ThanNhan { ESSN = "123456789", DependentName = "Elizabeth" });
        company.Dependents.Add(new ThanNhan { ESSN = "123456789", DependentName = "Michael" });
        company.Dependents.Add(new ThanNhan { ESSN = "333445555", DependentName = "Alice" });
        company.Dependents.Add(new ThanNhan { ESSN = "333445555", DependentName = "Joy" });
        company.Dependents.Add(new ThanNhan { ESSN = "333445555", DependentName = "Theodore" });
        company.Dependents.Add(new ThanNhan { ESSN = "987654321", DependentName = "Abner" });

        return company;
    }
}

/// <summary>
/// Company data structure containing employees and dependents
/// </summary>
[XmlRoot("Company")]
public class Company
{
    [XmlArray("Employees")]
    [XmlArrayItem("NhanVien")]
    public List<NhanVien> Employees { get; set; } = new List<NhanVien>();

    [XmlArray("Dependents")]
    [XmlArrayItem("ThanNhan")]
    public List<ThanNhan> Dependents { get; set; } = new List<ThanNhan>();
}

/// <summary>
/// Employee class (NhanVien)
/// </summary>
public class NhanVien
{
    [XmlElement("SSN")]
    public string SSN { get; set; } = "";

    [XmlElement("FName")]
    public string FName { get; set; } = "";

    [XmlElement("LName")]
    public string LName { get; set; } = "";

    [XmlElement("BDate")]
    public DateTime BDate { get; set; }

    [XmlElement("Salary")]
    public decimal Salary { get; set; }
}

/// <summary>
/// Dependent class (ThanNhan)
/// </summary>
public class ThanNhan
{
    [XmlElement("ESSN")]
    public string ESSN { get; set; } = "";

    [XmlElement("DependentName")]
    public string DependentName { get; set; } = "";
}
