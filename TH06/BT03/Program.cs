using LibBT01;

namespace BT03
{
    class Program
    {
        static void Main()
        {
            IKeyValueTable<string, string> table = new CountryCitiesTable();
            table.PrintTable();

            Console.Write("\nEnter a country key (UK, USA, India): ");
            var key = Console.ReadLine();

            Console.WriteLine();
            if (table.TryGet(key ?? "", out var cities))
                Console.WriteLine($"Cities in {key}: {cities}");
            else
                Console.WriteLine("Country not found.");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
