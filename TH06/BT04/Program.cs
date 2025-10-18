using LibBT01;

namespace BT04
{
    class Program
    {
        static void Main()
        {
            var employees = new List<Employee>
            {
                new() { Name = null,  NullLabel = "Employee" },
                new() { Name = "Tu"   },
                new() { Name = "Kieu" },
                new() { Name = "Chau" },
                new() { Name = null,  NullLabel = "Name" },
                new() { Name = "Long" }
            };

            Console.WriteLine("Initial employee list:");
            employees.ForEach(e => e.DisplayInfo());
            Console.WriteLine();

            employees.Sort(); // IComparable
            Console.WriteLine("Sorted with IComparable implementation:");
            employees.ForEach(e => e.DisplayInfo());
            Console.WriteLine();

            employees.Sort(new EmployeeComparer()); // IComparer
            Console.WriteLine("Sorted with IComparer implementation:");
            employees.ForEach(e => e.DisplayInfo());

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
