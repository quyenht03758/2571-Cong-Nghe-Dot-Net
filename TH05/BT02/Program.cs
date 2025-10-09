using LibBT01;

namespace BT02
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("============================");
            Console.WriteLine("       BOOK INFORMATION     ");
            Console.WriteLine("============================");
            Console.ResetColor();

            // Display book information
            TVBT book = new TVBT("IT124", "C# 11 and .NET 7", 2022, "Packt Publishing Ltd", 819);
            book.DisplayInformation();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n============================");
            Console.WriteLine("     ENUMERATION EXAMPLE    ");
            Console.WriteLine("============================");
            Console.ResetColor();

            foreach (TVBT.Days day in Enum.GetValues(typeof(TVBT.Days)))
            {
                Console.WriteLine($"- {day}");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n============================");
            Console.WriteLine("     COLLECTION EXAMPLE     ");
            Console.WriteLine("============================");
            Console.ResetColor();

            var family = TVBT.GetFamilyData();
            Console.WriteLine("Hung has {0} children:", family["Hung"].Count);
            foreach (var child in family["Hung"])
            {
                Console.WriteLine($"> {child}");
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n============================");
            Console.WriteLine(" PARAMETER PASSING EXAMPLE  ");
            Console.WriteLine("============================");
            Console.ResetColor();

            int a = 10, b = 20, c;
            TVBT.PassParameters(a, ref b, out c);
            Console.WriteLine($"Before: a = 10, b = 20, c = 30");
            Console.WriteLine($"After: a = {a}, b = {b}, c = {c}");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n============================");
            Console.WriteLine("         END OF PROGRAM     ");
            Console.WriteLine("============================");
            Console.ResetColor();
        }
    }
}
